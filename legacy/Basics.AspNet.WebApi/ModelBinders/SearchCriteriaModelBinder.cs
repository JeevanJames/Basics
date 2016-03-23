using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

using Basics.Models;

namespace Basics.AspNet.WebApi.ModelBinders
{
    public sealed class SearchCriteriaModelBinder : IModelBinder
    {
        bool IModelBinder.BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            //Check if the model type is a search criteria.
            Type filterFieldType, sortFieldType;
            if (!IsSearchCriteria(bindingContext.ModelType, out filterFieldType, out sortFieldType))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Should be a SearchCriteria instance.");
                return false;
            }

            //Create an instance of the search criteria.
            object criteria = Activator.CreateInstance(bindingContext.ModelType);
            bindingContext.Model = criteria;

            //Create all the query string handlers and execute them to populate the search criteria instance.
            var handlers = new List<QueryStringHandler>(4) {
                new TopQueryStringHandler(bindingContext),
                new SkipQueryStringHandler(bindingContext)
            };
            if (sortFieldType != null)
                handlers.Add(new OrderByQueryStringHandler(bindingContext, sortFieldType));
            if (filterFieldType != null)
                handlers.Add(new FilterQueryStringHandler(bindingContext, filterFieldType));
            return handlers.All(handler => handler.Resolve());
        }

        private static bool IsSearchCriteria(Type modelType, out Type filterField, out Type sortField)
        {
            Type type = modelType.BaseType;
            while (type != null && type != typeof(object))
            {
                if (type == typeof(SearchCriteria))
                {
                    filterField = sortField = null;
                    return true;
                }
                if (type.IsGenericType)
                {
                    Type typeDefinition = type.GetGenericTypeDefinition();
                    if (typeDefinition == typeof(SearchCriteria<>) || typeDefinition == typeof(SearchCriteria<,>))
                    {
                        Type[] genericArguments = type.GetGenericArguments();
                        if (genericArguments.Length == 1)
                        {
                            filterField = null;
                            sortField = genericArguments[0];
                        } else
                        {
                            filterField = genericArguments[0];
                            sortField = genericArguments[1];
                        }
                        return true;
                    }
                }
                type = type.BaseType;
            }
            filterField = sortField = null;
            return false;
        }
    }



    public sealed class SearchCriteriaAttribute : ModelBinderAttribute
    {
        public SearchCriteriaAttribute() : base(typeof(SearchCriteriaModelBinder))
        {
        }

        public override IEnumerable<ValueProviderFactory> GetValueProviderFactories(HttpConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            return base.GetValueProviderFactories(configuration)
                .OfType<IUriValueProviderFactory>()
                .Cast<ValueProviderFactory>();
        }
    }

    internal abstract class QueryStringHandler
    {
        private readonly ModelBindingContext _bindingContext;

        private readonly string _queryStringName;
        private readonly string _queryStringValue;

        private readonly object _criteria;
        private readonly PropertyInfo _criteriaProperty;

        internal QueryStringHandler(ModelBindingContext bindingContext, string queryStringName, string propertyName)
        {
            _bindingContext = bindingContext;

            _queryStringName = queryStringName;
            _queryStringValue = _bindingContext.ValueProvider.GetValue(_queryStringName)?.AttemptedValue;

            _criteria = bindingContext.Model;
            _criteriaProperty = _criteria.GetType().GetProperty(propertyName);
            if (_criteriaProperty == null)
            {
                throw new ArgumentException(
                    $"Cannot find property named {propertyName} in type {_criteria.GetType().FullName}.",
                    nameof(propertyName));
            }
        }

        internal bool Resolve()
        {
            //If it is null, it was not specified in the URL, so just get out.
            if (_queryStringValue == null)
                return true;
            try
            {
                //Get the property value from the model.
                object value = _criteriaProperty.GetValue(_criteria, null);

                //Call InternalResolve to perform the actual resolution for the property.
                object resolvedValue = ResolveValue(_queryStringValue, value);

                //Set the property value from the returned value, if SetValueAfterResolve is true (the default).
                //For collections and read-only properties, we do not want to set the value.
                if (SetValueAfterResolve)
                    _criteriaProperty.SetValue(_criteria, resolvedValue);

                return true;
            } catch (QueryStringHandlerException ex)
            {
                _bindingContext.ModelState.AddModelError(_queryStringName, ex.Message);
                return false;
            }
        }

        /// <summary>
        ///     Given the query string value and the initial property value, return the final property value.
        /// </summary>
        /// <param name="qsValue">The query string value.</param>
        /// <param name="propertyValue">The initial property value.</param>
        /// <returns>The resolved property value.</returns>
        protected abstract object ResolveValue(string qsValue, object propertyValue);

        /// <summary>
        ///     Specifies whether the resolved query string value should be assigned to the search criteria property after the
        ///     resolution. Normally, this would be true, unless you want to perform some other action, such as when the property
        ///     is a collection and you want to add resolved values to it.
        /// </summary>
        protected virtual bool SetValueAfterResolve => true;

        /// <summary>
        ///     Shortcut method to throw an exception while resolving query string values.
        /// </summary>
        /// <param name="message">The exception message. This will be added to the model state as a model error.</param>
        /// <exception cref="QueryStringHandlerException">Always thrown by this method.</exception>
        protected void Error(string message)
        {
            throw new QueryStringHandlerException(message);
        }
    }

    internal sealed class QueryStringHandlerException : Exception
    {
        internal QueryStringHandlerException(string message) : base(message)
        {
        }
    }

    internal abstract class QueryStringHandler<T> : QueryStringHandler
    {
        internal QueryStringHandler(ModelBindingContext bindingContext, string queryStringName, string propertyName)
            : base(bindingContext, queryStringName, propertyName)
        {
        }

        protected sealed override object ResolveValue(string qsValue, object propertyValue)
        {
            T typedValue = propertyValue != null ? (T)propertyValue : default(T);
            return ResolveTypedValue(qsValue, typedValue);
        }

        protected abstract T ResolveTypedValue(string qsValue, T propertyValue);
    }

    internal abstract class PaginationQueryStringHandler : QueryStringHandler<int?>
    {
        internal PaginationQueryStringHandler(ModelBindingContext bindingContext, string queryStringName,
            string propertyName) : base(bindingContext, queryStringName, propertyName)
        {
        }

        protected sealed override int? ResolveTypedValue(string qsValue, int? propertyValue)
        {
            if (string.IsNullOrWhiteSpace(qsValue))
                return null;
            int integer;
            if (!int.TryParse(qsValue, out integer))
                Error($"Invalid value '{qsValue}'. Should be a number");
            if (integer < 0)
                Error($"Invalid value '{qsValue}'. Should be a positive number");
            return integer;
        }
    }

    internal sealed class TopQueryStringHandler : PaginationQueryStringHandler
    {
        internal TopQueryStringHandler(ModelBindingContext bindingContext) : base(bindingContext, "$top", "RecordCount")
        {
        }
    }

    internal sealed class SkipQueryStringHandler : PaginationQueryStringHandler
    {
        internal SkipQueryStringHandler(ModelBindingContext bindingContext)
            : base(bindingContext, "$skip", "StartRecord")
        {
        }
    }

    internal sealed class OrderByQueryStringHandler : QueryStringHandler<IList>
    {
        private readonly Type _sortFieldType;

        internal OrderByQueryStringHandler(ModelBindingContext bindingContext, Type sortFieldType)
            : base(bindingContext, "$orderby", "SortSpecs")
        {
            _sortFieldType = sortFieldType;
        }

        protected override IList ResolveTypedValue(string orderByQueryString, IList sortSpecs)
        {
            string[] specs = orderByQueryString.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string spec in specs)
            {
                Match match = SpecPattern.Match(spec);
                if (!match.Success)
                    Error($"Invalid format - '{spec}'. Should be of the form '<field> [asc|desc]'.");
                string fieldName = match.Groups[1].Value;
                string sortOrder = string.IsNullOrWhiteSpace(match.Groups[2].Value) ? "asc" : match.Groups[2].Value;
                sortSpecs.Add(CreateSortSpec(fieldName, sortOrder));
            }
            return sortSpecs;
        }

        private object CreateSortSpec(string fieldName, string sortOrder)
        {
            FieldInfo fieldInfo = _sortFieldType.GetField(fieldName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
                Error($"Cannot find a sort field named {fieldName}.");
            SortOrder order = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? SortOrder.Ascending
                : SortOrder.Descending;
            Type sortSpecType = typeof(SortSpec<>).MakeGenericType(_sortFieldType);
            object sortSpec = Activator.CreateInstance(sortSpecType, fieldInfo.GetValue(null), order);
            return sortSpec;
        }

        protected override bool SetValueAfterResolve => false;

        private static readonly Regex SpecPattern = new Regex(@"^\s*([\w_]+)(?:\s+(asc|desc))?\s*$",
            RegexOptions.IgnoreCase);
    }

    internal sealed class FilterQueryStringHandler : QueryStringHandler<IList>
    {
        private readonly Type _filterFieldType;

        internal FilterQueryStringHandler(ModelBindingContext bindingContext, Type filterFieldType)
            : base(bindingContext, "$filter", "Filters")
        {
            _filterFieldType = filterFieldType;
        }

        protected override IList ResolveTypedValue(string filterQueryString, IList filters)
        {
            string[] specs = filterQueryString.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string spec in specs)
            {
                Match match = SpecPattern.Match(spec);
                if (!match.Success)
                    Error($"Invalid format - '{spec}'. Should be of the form '<field> <eq|ne|gt|ge|lt|le|lk> <value>'.");
                string fieldName = match.Groups[1].Value;
                string operand = match.Groups[2].Value;
                string value = match.Groups[3].Value;
                filters.Add(CreateFilterCriteria(fieldName, operand, value));
            }
            return filters;
        }

        private object CreateFilterCriteria(string fieldName, string operand, string value)
        {
            FieldInfo fieldInfo = _filterFieldType.GetField(fieldName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
                Error($"Cannot find a filter field named {fieldName}.");
            FilterOperation operation;
            if (!OperandMappings.TryGetValue(operand, out operation))
                Error($"Unrecognized operation '{operand}.");
            if (operation == FilterOperation.Like && !string.IsNullOrWhiteSpace(value))
                value = value.Replace('*', '%');
            Type filterCriteriaType = typeof(FilterCriteria<>).MakeGenericType(_filterFieldType);
            object filterCriteria = Activator.CreateInstance(filterCriteriaType, fieldInfo.GetValue(null), operation,
                value);
            return filterCriteria;
        }

        protected override bool SetValueAfterResolve => false;

        private static readonly Regex SpecPattern = new Regex(@"^\s*([\w_]+)\s+(eq|ne|gt|ge|lt|le|lk)\s+(.+)\s*$",
            RegexOptions.IgnoreCase);

        private static readonly Dictionary<string, FilterOperation> OperandMappings =
            new Dictionary<string, FilterOperation>(StringComparer.OrdinalIgnoreCase) {
                ["eq"] = FilterOperation.Equals,
                ["ne"] = FilterOperation.DoesNotEqual,
                ["gt"] = FilterOperation.GreaterThan,
                ["ge"] = FilterOperation.GreaterThanOrEqual,
                ["lt"] = FilterOperation.LessThan,
                ["le"] = FilterOperation.LessThanOrEqual,
                ["lk"] = FilterOperation.Like
            };
    }
}
