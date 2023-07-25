using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
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

        public IActionResult Products(string subcategory, int page = 1)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                IProducts products = new ProductsRepository(db);

                CurrentPageIndex = page;
                int firstPageIndex = 1;
                int lastPageIndex = 21;
                if (page > pagesCount * pageMultiplier)
                {
                    firstPageIndex = (pagesCount + 1) * (pageMultiplier - 1);
                    lastPageIndex = pagesCount * pageMultiplier;
                    pageMultiplier++;
                }

                Subcategory _subcategory = subcategories.FindSetByCondition(s => s.Name.ToLower() == subcategory).FirstOrDefault();
                IEnumerable<Product> _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page).ToList();

                return View(new ProductsCatalogViewModel { 
                    Subcategory = _subcategory, 
                    Products = _products, 
                    FirstPaginationIndex = firstPageIndex, 
                    LastPaginationIndex = lastPageIndex 
                });
            }
        }
    }
}
