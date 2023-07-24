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
        public int productsAtPage = 20;
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

                int firstProductIndex = (productsAtPage + 1) * (page - 1);

                Subcategory _subcategory = subcategories.FindSetByCondition(s => s.Name.ToLower() == subcategory).FirstOrDefault();
                IEnumerable<Product> _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Id >= 0).ToList();
                return View(new ProductsCatalogViewModel { Subcategory = _subcategory, Products = _products });
            }
        }
    }
}
