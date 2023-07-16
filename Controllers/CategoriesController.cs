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
                return View(categories.GetAllWithInclude(c => c.PageName == "Hardware").ToList().FirstOrDefault());
            }
        }
        public IActionResult Laptops()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Laptops").ToList().FirstOrDefault());
            }
        }
        public IActionResult Smartphones()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Smartphones").ToList().FirstOrDefault());
            }
        }
        public IActionResult Monitors()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(categories.GetAllWithInclude(c => c.PageName == "Monitors").ToList().FirstOrDefault());
            }
        }
    }
}
