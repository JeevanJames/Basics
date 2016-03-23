using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;
using System.Web.Http.ValueProviders;

namespace Basics.AspNet.WebApi.ModelBinders
{
    public class ListModelBinder : IModelBinder
    {
        bool IModelBinder.BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            string value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;
            IReadOnlyList<string> items = SplitValue(value);

            Type listType = GetListType(bindingContext.ModelType);
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType), items.Count);
            foreach (object item in ConvertItems(items, listType))
                list.Add(item);

            bindingContext.Model = list;
            return true;
        }

        private static Type GetListType(Type modelType)
        {
            Type modelTypeDefinition = modelType.GetGenericTypeDefinition();
            return typeof(List<>).GetInterfaces().Any(intf =>
                intf.IsGenericType && intf.GetGenericArguments().Length == 1 &&
                    intf.GetGenericTypeDefinition() == modelTypeDefinition)
                ? modelType.GetGenericArguments()[0]
                : null;
        }

        protected virtual IReadOnlyList<string> SplitValue(string value) =>
            value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        protected virtual IEnumerable<object> ConvertItems(IEnumerable<string> items, Type itemType)
        {
            Converter<string, object> converter = GetConverter(itemType);
            return items.Select(item => converter(item));
        }

        protected virtual Converter<string, object> GetConverter(Type itemType)
        {
            if (itemType == typeof(string))
                return s => s;

            TypeConverter typeConverter = TypeDescriptor.GetConverter(itemType);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                throw new Exception($"Type converter for {itemType.FullName} cannot convert from a string.");
            return s => typeConverter.ConvertFromString(s);
        }
    }

    public sealed class AsListAttribute : ModelBinderAttribute
    {
        public AsListAttribute() : base(typeof(ListModelBinder))
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

    public sealed class ListRouteConstraint : IHttpRouteConstraint
    {
        private readonly Type _type;

        public ListRouteConstraint(string typeName)
        {
            _type = Type.GetType(typeName);
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            if (_type == null)
                return false;

            object value;
            if (!values.TryGetValue(parameterName, out value))
                return false;

            var parameter = (string)value;
            IReadOnlyList<string> items = SplitValue(parameter);
            return CanConvertItems(items);
        }

        private static IReadOnlyList<string> SplitValue(string value) =>
            value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        private bool CanConvertItems(IEnumerable<string> items)
        {
            Converter<string, object> converter = GetConverter();
            if (converter == null)
                return false;
            try
            {
                foreach (string item in items)
                    converter(item);
            } catch (FormatException)
            {
                return false;
            } catch (Exception ex) when (ex.InnerException is FormatException)
            {
                return false;
            }
            return true;
        }

        private Converter<string, object> GetConverter()
        {
            if (_type == typeof(string))
                return s => s;

            TypeConverter typeConverter = TypeDescriptor.GetConverter(_type);
            if (!typeConverter.CanConvertFrom(typeof(string)))
                return null;
            return s => typeConverter.ConvertFromString(s);
        }
    }
}
