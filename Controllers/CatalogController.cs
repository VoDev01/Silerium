using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Query;
using Silerium.Models.Repositories;
using Silerium.ViewModels;

namespace Silerium.Controllers
{
    public class CatalogController : Controller
    {
        private string connectionString;
        public ILogger<CatalogController> logger;
        public static readonly int ProductsAtPage = 20;
        private int pagesCount = 10;
        private int pageMultiplier = 1;
        public static int CurrentPageIndex { get; set; } = 1;
        public CatalogController(ILogger<CatalogController> logger)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
            this.logger = logger;
        }
        public IActionResult Product(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                IProducts products = new ProductsRepository(db);
                Product product = products.
                    GetAllWithInclude(p => p.Subcategory).
                    Include(p => p.Specifications).
                    Include(p => p.Images).
                    Where(p => p.Id == id).FirstOrDefault();

                return View(new ProductViewModel { Product = product });
            }
        }
        public IActionResult Products(string subcategory_name, string category_name, string product_name, string sort_order, int page = 1)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                IProducts products = new ProductsRepository(db);

                CurrentPageIndex = page;
                int firstPageIndex = 1;
                int lastPageIndex = 10;
                if (page > pagesCount * pageMultiplier)
                {
                    firstPageIndex = (pagesCount + 1) * (pageMultiplier - 1);
                    lastPageIndex = pagesCount * pageMultiplier;
                    pageMultiplier++;
                }

                Subcategory _subcategory = subcategories.FindSetByCondition(s => s.Name.ToLower() == subcategory_name).FirstOrDefault();
                List<Product> _products = (List<Product>)HttpContext.Items["products"];

                return View(new ProductsCatalogViewModel { 
                    Subcategory = _subcategory, 
                    Products = _products,
                    ProductsQuery = new ProductsQuery(),
                    FirstPaginationIndex = firstPageIndex, 
                    LastPaginationIndex = lastPageIndex 
                });
            }
        }
        [HttpPost]
        public IActionResult Products(string subcategory_name, string sort_order, int page = 1)
        {
            return RedirectToAction("Products", "Catalog", new
            {
                subcategory_name,
                sort_order,
                page
            });
        }
    }
}
