using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

#if !DEBUG
using Basics.Domain;
#endif

namespace Basics.AspNet.WebApi
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(HttpActionExecutedContext context,
            CancellationToken cancellationToken)
        {
            string correlationId = await LogException(context.Exception);

#if DEBUG
            string description = context.Exception.ToString();
#else
            var domainException = context.Exception as DomainException;
            string description = domainException != null
                ? string.Format(SpecificMessageFormat, domainException.Message, correlationId)
                : string.Format(GenericMessageFormat, correlationId);
#endif

            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                ReasonPhrase = $"Internal server error. Code {correlationId}",
                Content = new StringContent(description)
            };
            if (!string.IsNullOrWhiteSpace(CorrelationIdResponseHeaderKey))
                response.Headers.Add(CorrelationIdResponseHeaderKey, correlationId);
            throw new HttpResponseException(response);
        }

        protected virtual Task<string> LogException(Exception ex)
        {
            return Task.FromResult(Guid.NewGuid().ToString("D"));
        }

        protected virtual string GenericMessageFormat =>
            "Internal server error. Contact your administrator with error ID {0}";

        protected virtual string SpecificMessageFormat => "{0}.Contact your administrator with error ID {1}";

        protected virtual string CorrelationIdResponseHeaderKey => null;
    }

    public class GlobalExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// When overridden in a derived class, handles the exception synchronously.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            base.Handle(context);
        }

        /// <summary>
        /// When overridden in a derived class, handles the exception asynchronously.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous exception handling operation.
        /// </returns>
        /// <param name="context">The exception handler context.</param><param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            return base.HandleAsync(context, cancellationToken);
        }

        /// <summary>
        /// Determines whether the exception should be handled.
        /// </summary>
        /// <returns>
        /// true if the exception should be handled; otherwise, false.
        /// </returns>
        /// <param name="context">The exception handler context.</param>
        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return base.ShouldHandle(context);
        }
    }
    public static class ConfigurationExtensions
    {
        public static HttpConfiguration UseGlobalExceptionFilter<TFilter>(this HttpConfiguration configuration, TFilter filter)
            where TFilter : IExceptionFilter
        {
            configuration.Filters.Add(filter);
            return configuration;
        }

        public static HttpConfiguration UseGlobalExceptionHandler<THandler>(this HttpConfiguration configuration, THandler handler)
            where THandler : IExceptionHandler
        {
            configuration.Services.Add(typeof(IExceptionHandler), handler);
            return configuration;
        }
    }
}
