using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Basics.AspNet.WebApi
{
    public sealed class GlobalPrefixProvider : DefaultDirectRouteProvider
    {
        private static readonly string GlobalPrefix;

        static GlobalPrefixProvider()
        {
            CustomAttributeData attributeData = Assembly
                .GetExecutingAssembly()
                .CustomAttributes
                .FirstOrDefault(data => data.AttributeType == typeof(GlobalRoutePrefixAttribute));
            GlobalPrefix = (string) attributeData?.ConstructorArguments[0].Value;
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            string existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            return existingPrefix == null ? GlobalPrefix : $"{GlobalPrefix}/{existingPrefix}";
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class GlobalRoutePrefixAttribute : Attribute
    {
        public GlobalRoutePrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; }
    }
}