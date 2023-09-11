
namespace Silerium.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class JWTAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string? token = httpContext.Session.GetString("access_token");
            if (token != null)
            {
                httpContext.Request.Headers.Add("Authorization", "Bearer " + token);
            }
            await _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JWTAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJWTAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTAuthorizationMiddleware>();
        }
    }
}
