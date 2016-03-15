using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;

using Basics.Containers;
using Basics.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Basics.AspNet.WebApi
{
    /// <summary>
    ///     Set of misc extension methods that are useful for Web API/
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Registers all controller classes in the specified assembly with the container builder.
        /// </summary>
        /// <param name="builder">The container builder where the controller classes will be registered.</param>
        /// <param name="controllersAssembly">The assembly containing the controller classes to register/</param>
        public static void RegisterApiControllers(this IContainerBuilder builder, Assembly controllersAssembly)
        {
            builder.RegisterAssembly(controllersAssembly, type =>
                typeof (IHttpController).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass);
        }

        public static void SetDefaults(this HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new BasicsDependencyResolver(container);

            config.MapHttpAttributeRoutes(new GlobalPrefixProvider());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Default;

            config.Formatters.Clear();
            var jsonFormatter = new JsonMediaTypeFormatter {
                SerializerSettings = {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                }
            };
            jsonFormatter.SerializerSettings.Converters.Add(new UnixEpochDateTimeConverter());
            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.Add(jsonFormatter);
        }

        public static UserContext ToUserContext(this IPrincipal principal)
        {
            string[] permissions = (principal as ClaimsPrincipal)?.Claims
                .Where(claim => claim.Type == BasicsClaimTypes.Permission)
                .Select(claim => claim.Value)
                .ToArray();
            if (permissions == null)
            {
                throw new ArgumentException("IPrincipal type to convert to UserContext should be a ClaimsPrincipal.",
                    nameof(principal));
            }
            return new UserContext(principal.Identity.Name, permissions);
        }
    }
}
