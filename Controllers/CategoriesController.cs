using Microsoft.AspNetCore.Mvc;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.ViewModels;

namespace Silerium.Controllers
{
    public class CategoriesController : Controller
    {
        private string connectionString;
        public CategoriesController()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
        }
        public IActionResult Index()
        {
            using(var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAll().ToList());
            }
        }
        public IActionResult Hardware()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Hardware").FirstOrDefault());
            }
        }
        public IActionResult Laptops()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Laptops").FirstOrDefault());
            }
        }
        public IActionResult Smartphones()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Smartphones").FirstOrDefault());
            }
        }
        public IActionResult Monitors()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Monitors").FirstOrDefault());
            }
        }
    }
}
