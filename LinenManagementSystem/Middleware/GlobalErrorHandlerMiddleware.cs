using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace LinenManagementSystem.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                    // Log the exception
                    _logger.LogError($"Something went wrong: {ex}");

                    // Handle the exception and return a response
                    await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.Clear();

            // Set the status code to 500 (Internal Server Error)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


            // Set the response content type
            context.Response.ContentType = "text/plain";

            // Return a generic error message to avoid leaking sensitive information
            return context.Response.WriteAsync("Oops! Something went wrong, please call support.");
        }
    }

    

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandlerMiddleware>();
        }

       
    }
   
}
