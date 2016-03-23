using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Basics.AspNet.WebApi.ModelBinders
{
    /// <summary>
    ///     Allows you to use the <see cref="QueryStringNameAttribute" /> attribute on properties of your model to specify the
    ///     query string name they will be bound to.
    /// </summary>
    public sealed class NamedQueryStringModelBinder : IModelBinder
    {
        bool IModelBinder.BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            object model = Activator.CreateInstance(bindingContext.ModelType);
            foreach (KeyValuePair<string, ModelMetadata> metadata in bindingContext.PropertyMetadata)
            {
                PropertyInfo property = bindingContext.ModelType.GetProperty(metadata.Key);
                QueryStringNameAttribute qsAttribute = property.GetCustomAttributes(false)
                    .OfType<QueryStringNameAttribute>()
                    .FirstOrDefault();
                string qsParameter = qsAttribute != null ? qsAttribute.Name : metadata.Key;
                string value = bindingContext.ValueProvider.GetValue(qsParameter)?.AttemptedValue;
                property.SetValue(model, ConvertFromString(value, property.PropertyType));
            }
            //TODO: Figure out how to validate data annotations
            //bindingContext.ValidationNode.ValidateAllProperties = true;
            //bindingContext.ValidationNode.Validate(actionContext, null);
            bindingContext.Model = model;
            return true;
        }

        private static object ConvertFromString(string sourceValue, Type destinationType)
        {
            if (destinationType == typeof(string))
                return sourceValue;
            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            if (!converter.CanConvertFrom(typeof(string)))
                throw new Exception($"Type converter for {destinationType.FullName} cannot convert from a string.");
            return converter.ConvertFromString(sourceValue);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public sealed class NamedQueryStringAttribute : ModelBinderAttribute
    {
        public NamedQueryStringAttribute() : base(typeof(NamedQueryStringModelBinder))
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

    /// <summary>
    ///     Applied to the model properties to indicate the name of the query string they are bound to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class QueryStringNameAttribute : Attribute
    {
        public QueryStringNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
