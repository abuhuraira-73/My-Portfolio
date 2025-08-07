using Microsoft.AspNetCore.Http;
using My_CV_3.Models;
using My_CV_3.Services;
using System.Threading.Tasks;

namespace My_CV_3.Middleware
{
    public class VisitorTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public VisitorTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, VisitorService visitorService)
        {
            // Skip tracking for static files, API calls, etc.
            if (!context.Request.Path.Value.StartsWith("/css") && 
                !context.Request.Path.Value.StartsWith("/js") && 
                !context.Request.Path.Value.StartsWith("/lib") && 
                !context.Request.Path.Value.StartsWith("/images") &&
                !context.Request.Path.Value.Contains("favicon"))
            {
                var visitor = new Visitor
                {
                    PageUrl = context.Request.Path.Value,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = context.Request.Headers["User-Agent"].ToString()
                };

                // Record the visit asynchronously (don't wait for it to complete)
                _ = visitorService.RecordVisitAsync(visitor);
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

    // Extension method for registering the middleware
    public static class VisitorTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseVisitorTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VisitorTrackingMiddleware>();
        }
    }
}