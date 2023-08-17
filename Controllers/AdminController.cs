﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
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
        private ILogger<AdminController> logger;
        public AdminController(ILogger<AdminController> logger)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
            this.logger = logger;
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
                ICategories categories = new CategoriesRepository(db);
                List<Category> categoriesList = categories.GetAllWithInclude(c => c.Subcategories).ToList();
                return View(categoriesList);
            }
        }

        // GET: AdminController/Create
        public IActionResult CreateCategory()
        {
            return View(new CategoryViewModel());
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
                            $"category_{num}{Path.GetExtension(categoryVM.FormImage.FileName)}"
                            )))
                    {
                        categoryVM.FormImage.CopyTo(fstream);
                        categoryVM.Category.Image = "\\" + Path.Combine("images", "categories", Path.GetFileName(fstream.Name));
                        fstream.Flush();
                    }
                    categories.Create(categoryVM.Category);
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
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                Category category = categories.GetByID(id-1);
                CategoryViewModel categoryVM = new CategoryViewModel();
                categoryVM.Category = category;
                return View(categoryVM);
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
                            "wwwroot",
                            "images",
                            "categories",
                            $"category_{categoryVM.Category.Id}{Path.GetExtension(categoryVM.FormImage.FileName)}"
                            )))
                    {
                        categoryVM.FormImage.CopyTo(fstream);
                        categoryVM.Category.Image = "\\" + Path.Combine("images", "categories", Path.GetFileName(fstream.Name));
                        fstream.Flush();
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
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                Category category = categories.GetAllWithInclude(c => c.Subcategories).ElementAt(id-1);
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
                    ICategories categories = new CategoriesRepository(db);
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    categories.Delete(category);
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
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                List<Subcategory> subcategoriesList = subcategories.GetAllWithInclude(c => c.Category).ToList();
                return View(subcategoriesList);
            }
        }

        // GET: AdminController/Create
        public IActionResult CreateSubcategory()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ICategories categories = new CategoriesRepository(db);
                return View(new SubcategoryViewModel { Subcategory = new Subcategory(), Categories = categories.GetAll().ToList()});
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
                            $"subcategory_{num}{Path.GetExtension(subcategoryVM.FormImage.FileName)}"
                            )))
                    {
                        subcategoryVM.FormImage.CopyTo(fstream);
                        subcategoryVM.Subcategory.Image = "\\" + Path.Combine("images", "subcategories", Path.GetFileName(fstream.Name));
                        fstream.Flush();
                    }

                    subcategories.Create(subcategoryVM.Subcategory);
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
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                SubcategoryViewModel subcategoryVM = new SubcategoryViewModel { Subcategory = subcategories.GetByID(id - 1) };
                return View(subcategoryVM);
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
                            $"subcategory_{num}{Path.GetExtension(subcategoryVM.FormImage.FileName)}"
                            )))
                    {
                        subcategoryVM.FormImage.CopyTo(fstream);
                        subcategoryVM.Subcategory.Image = "\\" + Path.Combine("images", "subcategories", Path.GetFileName(fstream.Name));
                        fstream.Flush();
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
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
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
                    ISubcategories subcategories = new SubcategoriesRepository(db);
                    subcategories.Delete(subcategory);
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
            using (var db = new ApplicationDbContext(connectionString))
            {
                IProducts products = new ProductsRepository(db);
                List<Product> productsList = products.GetAllWithInclude(p => p.Subcategory).ToList();
                return View(productsList);
            }
        }
        // GET: AdminController/Create
        public IActionResult CreateProduct()
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
                        if (products.FindSetByCondition(p => p.Page.Id == CatalogController.CurrentPageIndex).Count() > CatalogController.productsAtPage)
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
                            $"product_{imageId}{productVM.Product.Id}{Path.GetExtension(formImage.FileName)}"
                            )))
                        {
                            formImage.CopyTo(fstream);
                            imagePath = "\\" + Path.Combine("images", "products", Path.GetFileName(fstream.Name));
                            fstream.Flush();
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
                    products.Create(productVM.Product);
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
        public IActionResult EditProduct(int id)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProducts products = new ProductsRepository(db);
                    Product product = products.GetByID(id - 1);
                    return View(product);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProducts products = new ProductsRepository(db);
                    Product productDB = products.GetByID(product.Id - 1);
                    productDB = product;
                    products.Save();
                    return View(product);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProducts products = new ProductsRepository(db);
                    Product product = products.GetByID(id - 1);
                    return View(product);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
        [HttpPost]
        public IActionResult DeletProductPost(int id)
        {
            try
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IProducts products = new ProductsRepository(db);
                    IPages pages = new PagesRepository(db);
                    Product product = products.GetByID(id - 1);
                    Page page = pages.FindSetByCondition(p => p.Products.Contains(product)).FirstOrDefault() 
                        ?? pages.FindSetByCondition(p => p.Id == CatalogController.CurrentPageIndex).FirstOrDefault();
                    products.Delete(product);
                    products.Save();
                    page.Products.Remove(product);
                    if (page.Products.Count <= 0)
                    {
                        pages.Delete(page);
                        pages.Save();
                    }
                    return RedirectToAction("Products", "Admin");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return View();
            }
        }
        public IActionResult Users()
        {
            using(var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                return View(users.GetAll().ToList());
            }
        }
        [HttpPost]
        public IActionResult UpdateUsers()
        {
            return RedirectToAction("Users", "Admin");
        }
        public IActionResult Orders()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IOrders orders = new OrdersRepository(db);
                IEnumerable<Order> orderModel = orders.GetAllWithInclude(o => o.User).Include(o => o.Product).ToList();
                return View(orderModel);
            }
        }
        [HttpPost]
        public IActionResult UpdateOrders()
        {
            return RedirectToAction("Orders", "Admin");
        }
        public IActionResult OrderDetails(string orderid)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IOrders orders = new OrdersRepository(db);
                var guid = new Guid(orderid);
                return View(orders.GetAllWithInclude(o => o.User).Include(o => o.Product).Where(u => u.OrderId == guid).ToList().FirstOrDefault());
            }
        }
    }
}
