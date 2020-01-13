using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Authentication;
namespace AuthService.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();
            var statusCode = exceptionType == typeof(AuthenticationException) 
                ? (int)HttpStatusCode.Unauthorized 
                : (int)HttpStatusCode.InternalServerError;

            context.Result = new ContentResult
            {
                Content = $"Internal Server Error: {context.Exception.Message}",
                ContentType = "text/plain",
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}

