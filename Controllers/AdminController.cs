using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.Services;
using Silerium.ViewModels.ProductsModels;
using System.Data;

namespace Silerium.Controllers
{
    public class AdminController : Controller
    {
        private string connectionString;
        private ILogger<AdminController> logger;
        private IAuthorizationService authorizationService;
        public AdminController(ILogger<AdminController> logger, IAuthorizationService authorizationService)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
            this.logger = logger;
            this.authorizationService = authorizationService;
        }

        public IActionResult NoPermissions(string permission)
        {
            return View(permission + ".");
        }
        // GET: AdminController
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Categories()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.View)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    List<Category> categoriesList = categories.GetAllWithInclude(c => c.Subcategories).ToList();
                    return View(categoriesList);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.View) });
            }
        }

        // GET: AdminController/Create
        public IActionResult CreateCategory()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                return View(new CategoryViewModel());
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(CategoryViewModel categoryVM)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    int num = categories.GetAll().Count();
                    using (var fstream = System.IO.File.Create(
                        Path.Combine(
                            Directory.GetCurrentDirectory(), 
                            "wwwroot",
                            "images",
                            "categories",
                            $"category_{num}.jpg"
                            )))
                    {
                        categoryVM.FormImage.CopyTo(fstream);
                        fstream.Flush();
                        categoryVM.Category.Image = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), fstream.Name);
                    }
                    categories.Add(categoryVM.Category);
                    categories.Save();
                    return RedirectToAction("Categories", "Admin");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public IActionResult EditCategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    Category category = categories.GetByID(id-1);
                    CategoryViewModel categoryVM = new CategoryViewModel();
                    categoryVM.Category = category;
                    return View(categoryVM);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(CategoryViewModel categoryVM)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    Category category = categories.GetByID(categoryVM.Category.Id-1);
                    int num = categories.GetAll().Count();
                    using (var fstream = System.IO.File.Create(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "/wwwroot/images/categories/",
                            $"category_{categoryVM.Category.Id}.jpg"
                            )))
                    {
                        categoryVM.FormImage.CopyTo(fstream);
                        fstream.Flush();
                        categoryVM.Category.Image = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), fstream.Name);
                    }
                    category = categoryVM.Category;
                    categories.Save();
                    return RedirectToAction("Categories", "Admin");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public IActionResult DeleteCategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    Category category = categories.GetAllWithInclude(c => c.Subcategories).ElementAt(id-1);
                    return View(category);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
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
                    ICategories categories = new CategoriesRepository(db);
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    categories.Remove(category);
                    categories.Save();
                    return RedirectToAction("Categories", "Admin");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
        public IActionResult Subcategories()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    List<Subcategory> subcategoriesList = subcategories.GetAllWithInclude(c => c.Category).ToList();
                    return View(subcategoriesList);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }

        // GET: AdminController/Create
        public IActionResult CreateSubcategory()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    return View(new SubcategoryViewModel { Subcategory = new Subcategory(), Categories = categories.GetAll().ToList()});
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSubcategory(SubcategoryViewModel subcategoryVM, int categoryid)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    ICategories categories = new CategoriesRepository(db);
                    Category category = categories.GetByID(categoryid - 1);

                    int num = subcategories.GetAll().Count();
                    subcategoryVM.Subcategory.Category = category;

                    using (var fstream = System.IO.File.Create(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "subcategories",
                            $"subcategory_{num}.jpg"
                            )))
                    {
                        subcategoryVM.FormImage.CopyTo(fstream);
                        fstream.Flush();
                        subcategoryVM.Subcategory.Image = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), fstream.Name);
                    }

                    subcategories.Add(subcategoryVM.Subcategory);
                    subcategories.Save();
                    return RedirectToAction("Subcategories", "Admin");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public IActionResult EditSubcategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    SubcategoryViewModel subcategoryVM = new SubcategoryViewModel { Subcategory = subcategories.GetByID(id - 1) };
                    return View(subcategoryVM);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSubcategory(SubcategoryViewModel subcategoryVM)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    Subcategory subcategory = subcategories.GetByID(subcategoryVM.Subcategory.Id - 1);
                    int num = subcategories.GetAll().Count();

                    using (var fstream = System.IO.File.Create(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "subcategories",
                            $"subcategory_{num}.jpg"
                            )))
                    {
                        subcategoryVM.FormImage.CopyTo(fstream);
                        fstream.Flush();
                        subcategoryVM.Subcategory.Image = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), fstream.Name);
                    }
                    subcategory = subcategoryVM.Subcategory;
                    subcategories.Save();
                    return RedirectToAction("Subcategories", "Admin");
                }
            }
            catch(Exception e) 
            {
                logger.LogError(e.Message); 
                return View(); 
            }
        }

        // GET: AdminController/Delete/5
        public IActionResult DeleteSubcategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    IEnumerable<Subcategory> _subcategories = subcategories.GetAllWithInclude(sc => sc.Category);
                    Subcategory subcategory = _subcategories.ElementAt(id - 1);
                    return View(subcategory);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
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
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    subcategories.Remove(subcategory);
                    subcategories.Save();
                    return RedirectToAction("Subcategories", "Admin");
                }
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
        public IActionResult Products()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProducts products = new ProductsRepository(db);
                    List<Product> productsList = products.GetAllWithInclude(p => p.Subcategory).ToList();
                    return View(productsList);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }
        // GET: AdminController/Create
        public IActionResult CreateProduct()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    ProductViewModel productViewModel = new ProductViewModel();
                    productViewModel.Product = new Product();
                    productViewModel.Subcategories = subcategories.GetAll().ToList();
                    return View(productViewModel);
                }
            }
            else
            {
                return RedirectToAction("NoPermissions", new { permission = RolesManagerService.GeneratePermissionForModel("Categories", PermissionType.Create) });
            }
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(ProductViewModel productVM, int product_subcategory)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProducts products = new ProductsRepository(db);
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    IPages pages = new PagesRepository(db);

                    if (pages.GetAll().Count() == 0)
                    {
                        productVM.Product.Page = new Page { Products = new List<Product>() { productVM.Product } };
                    }
                    else
                    {
                        if (products.Find(p => p.Page.Id == CatalogController.CurrentPageIndex).Count() > CatalogController.productsAtPage)
                            productVM.Product.Page = new Page { Products = new List<Product>() { productVM.Product } };
                        else
                            productVM.Product.Page = pages.GetByID(CatalogController.CurrentPageIndex);
                    }

                    int num = products.GetAll().Count();
                    int imageId = 1;

                    foreach (var formImage in productVM.FormImages)
                    {
                        string imagePath;
                        using (var fstream = System.IO.File.Create(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "products",
                            $"product_{imageId}{productVM.Product.Id}"
                            )))
                        {
                            formImage.CopyTo(fstream);
                            fstream.Flush();
                            imagePath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), fstream.Name);
                        }
                        productVM.Product.Images.Add(new ProductImage { Image = imagePath });
                        imageId++;
                    }
                    List<ProductSpecification> specifications = new List<ProductSpecification>();
                    foreach (var specification in HttpContext.Request.Query.Where(q => q.Key.Contains("spec_name")))
                    {
                        specifications.Add(new ProductSpecification { Name = specification.Key, Specification = specification.Value });
                    }
                    productVM.Product.Subcategory = subcategories.GetByID(product_subcategory - 1);
                    products.Add(productVM.Product);
                    products.Save();
                    return RedirectToAction("Products", "Admin");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
    }
}
