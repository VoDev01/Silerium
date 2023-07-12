using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.ViewModels;

namespace Silerium.Controllers
{
    public class AdminController : Controller
    {
        private string connectionString;

        public AdminController()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
        }

        // GET: AdminController
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Categories()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategory categories = new CategoryRepository(db);
                List<Category> categoriesList = categories.GetAllWithInclude(c => c.Subcategories).ToList();
                return View(categoriesList);
            }
        }

        // GET: AdminController/Create
        public IActionResult CreateCategory()
        {
            return View(new Category());
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategory categories = new CategoryRepository(db);
                    categories.Create(category);
                    categories.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public IActionResult EditCategory(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategory categories = new CategoryRepository(db);
                Category category = categories.GetByID(id-1);
                return View(category);
            }
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(Category category)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategory categories = new CategoryRepository(db);
                    Category categoryDB = categories.GetByID(category.Id-1);
                    categoryDB = category;
                    categories.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public IActionResult DeleteCategory(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategory categories = new CategoryRepository(db);
                IEnumerable<Category> _categories = categories.GetAllWithInclude(c => c.Subcategories);
                Category category = _categories.ElementAt(id-1);
                return View(category);
            }
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(Category category)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategory categories = new CategoryRepository(db);
                    ISubcategory subcategories = new SubcategoryRepository(db);
                    foreach(var subcategory in categories.GetByID(category.Id-1).Subcategories)
                    {
                        subcategories.Delete(subcategory);
                    }
                    subcategories.Save();
                    categories.Delete(category);
                    categories.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }
        public IActionResult Subcategories()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategory categories = new CategoryRepository(db);
                List<Category> categoriesList = categories.GetAllWithInclude(c => c.Subcategories).ToList();
                return View(categoriesList);
            }
        }

        // GET: AdminController/Create
        public IActionResult CreateSubcategory()
        {
            return View(new Subcategory());
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSubcategory(Subcategory subcategory)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategory categories = new SubcategoryRepository(db);
                    categories.Create(subcategory);
                    categories.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public IActionResult EditSubcategory(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategory subcategories = new SubcategoryRepository(db);
                Subcategory subcategory = subcategories.GetByID(id - 1);
                return View(subcategory);
            }
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSubcategory(Subcategory subcategory)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategory subcategories = new SubcategoryRepository(db);
                    Subcategory categoryDB = subcategories.GetByID(subcategory.Id - 1);
                    categoryDB = subcategory;
                    subcategories.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public IActionResult DeleteSubcategory(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategory subcategories = new SubcategoryRepository(db);
                IEnumerable<Subcategory> _subcategories = subcategories.GetAllWithInclude(sc => sc.Category);
                Subcategory subcategory = _subcategories.ElementAt(id - 1);
                return View(subcategory);
            }
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSubcategory(Subcategory subcategory)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategory subcategories = new SubcategoryRepository(db);
                    subcategories.Delete(subcategory);
                    subcategories.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }
        public IActionResult Products()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IProduct products = new ProductRepository(db);
                List<Product> productsList = products.GetAllWithInclude(p => p.Subcategory).ToList();
                return View(productsList);
            }
        }
        // GET: AdminController/Create
        public IActionResult CreateProduct()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategory subcategories = new SubcategoryRepository(db);
                ProductViewModel productViewModel = new ProductViewModel();
                productViewModel.Product = new Product();
                productViewModel.Subcategories = subcategories.GetAll().ToList();
                return View(productViewModel);
            }
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(ProductViewModel productViewModel)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProduct products = new ProductRepository(db);
                    products.Create(productViewModel.Product);
                    products.Save();
                    return RedirectToAction("Index", "Admin");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
