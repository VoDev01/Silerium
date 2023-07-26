using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Silerium.Data;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Silerium.Middlewares
{
    public class ProductsSortingMiddleware
    {
        private readonly string connectionString;
        private readonly RequestDelegate _next;

        public ProductsSortingMiddleware(RequestDelegate next, string connectionString)
        {
            _next = next;
            this.connectionString = connectionString;
        }

        public Task Invoke(HttpContext httpContext)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                IProducts products = new ProductsRepository(db);

                List <Product> _products = new List<Product>();
                int page = Convert.ToInt32(httpContext.Request.Query["page"]);

                switch (httpContext.Request.Query["sort_order"])
                {
                    case nameof(Models.Query.SortOrder.NAME_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.Name).ToList();
                        break;
                    case nameof(Models.Query.SortOrder.NAME_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.Name).ToList();
                        break;
                    case nameof(Models.Query.SortOrder.POP_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Include(p => p.Orders).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(Models.Query.SortOrder.POP_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Include(p => p.Orders).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(Models.Query.SortOrder.PRICE_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.PriceRub).ToList();
                        break;
                    case nameof(Models.Query.SortOrder.PRICE_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.PriceRub).ToList();
                        break;
                }

                httpContext.Items["products"] = _products;
                return _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ProductsSortingMiddlewareExtensions
    {
        public static IApplicationBuilder UseProductsSortingMiddleware(this IApplicationBuilder builder, string connectionString)
        {
            return builder.UseMiddleware<ProductsSortingMiddleware>(connectionString);
        }
    }
}
