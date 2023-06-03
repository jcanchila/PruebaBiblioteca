using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace PruebaIngresoBibliotecario.Api.Filters
{
    public class BibliotecaExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        public BibliotecaExceptionFilter(ILogger<Exception> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message, new[] { context.Exception.StackTrace });

            var error = new
            {
                Error = context.Exception.Message,
                TypeError = context.Exception.GetType().ToString()
            };

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new ObjectResult(error);
        }
    }
}
