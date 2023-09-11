using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sql;
using Silerium.Data;
using Microsoft.Data.SqlClient;

namespace Silerium.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HandleDuplicateRequestsMiddleware
    {
        private readonly string key;
        private readonly string alertTempDataKey;
        private readonly RequestDelegate _next;

        public HandleDuplicateRequestsMiddleware(RequestDelegate next, string key = "Idempotency-Token", string alertTempDataKey = "AlertCookie")
        {
            _next = next;
            this.key = key;
            this.alertTempDataKey = alertTempDataKey;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(httpContext.Request.Method == HttpMethod.Post.Method &&
                httpContext.Request.Headers.TryGetValue(key, out var values))
            {
                var token = values.FirstOrDefault();
                var database = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                var factory = httpContext.RequestServices.GetRequiredService<TempDataDictionaryFactory>();
                var tempData = factory.GetTempData(httpContext);

                try
                {
                    database.Requests.Add(new Models.Request
                    {
                        IdempotentToken = token
                    });
                    await database.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                    when (ex.InnerException is SqlException { ErrorCode : 544 })
                {
                    tempData[alertTempDataKey] =
                            "You somehow sent this message multiple time. " +
                            "Don't worry its safe, you can carry on." +
                            "warning";
                    tempData.Keep(alertTempDataKey);
                    httpContext.Response.Redirect("/", false);
                }
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HandleDuplicateRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseHandleDuplicateRequestsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HandleDuplicateRequestsMiddleware>();
        }
    }
}
