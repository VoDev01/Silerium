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
using Silerium.ViewModels.PermissionAuthorizationModels;
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
        [Area("Home")]
        public IActionResult Index()
        {
            return View();
        }
        [Area("CategoriesControl")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Categories()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Category", PermissionType.View)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Category", PermissionType.View);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
            }
        }
        // GET: AdminController/Create
        [Area("CategoriesControl")]
        public IActionResult CreateCategory()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Categories", PermissionType.Create)).Result.Succeeded)
            {
                return View(new CategoryViewModel());
            }
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Categories", PermissionType.Create);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
                            $"category_{num}{Path.GetExtension(categoryVM.FormImage.FileName)}"
                            )))
                    {
                        categoryVM.FormImage.CopyTo(fstream);
                        categoryVM.Category.Image = "\\" + Path.Combine("images", "categories", Path.GetFileName(fstream.Name));
                        fstream.Flush();
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
        [Area("CategoriesControl")]
        public IActionResult EditCategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Categories", PermissionType.Edit)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Categories", PermissionType.Edit);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("CategoriesControl")]
        public IActionResult DeleteCategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Categories", PermissionType.Delete)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Categories", PermissionType.Delete);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("SubcategoriesControl")]
        public IActionResult Subcategories()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Create)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.View);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
            }
        }
        [Area("SubcategoriesControl")]
        // GET: AdminController/Create
        public IActionResult CreateSubcategory()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Create)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    ICategories categories = new CategoriesRepository(db);
                    return View(new SubcategoryViewModel { Subcategory = new Subcategory(), Categories = categories.GetAll().ToList()});
                }
            }
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Create);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("SubcategoriesControl")]
        public IActionResult EditSubcategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Edit)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Edit);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("SubcategoriesControl")]
        public IActionResult DeleteSubcategory(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Delete)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Subcategories", PermissionType.Delete);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("ProductsControl")]
        public IActionResult Products()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.View)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.View);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
            }
        }
        [Area("ProductsControl")]
        // GET: AdminController/Create
        public IActionResult CreateProduct()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.Create)).Result.Succeeded)
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
                string permission = RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.Create);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("ProductsControl")]
        public IActionResult EditProduct(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.Edit)).Result.Succeeded)
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
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.Edit);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
        [Area("ProductsControl")]
        public IActionResult DeleteProduct(int id)
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.Delete)).Result.Succeeded)
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
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Products", PermissionType.Delete);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
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
                    Page page = pages.Find(p => p.Products.Contains(product)).FirstOrDefault() 
                        ?? pages.Find(p => p.Id == CatalogController.CurrentPageIndex).FirstOrDefault();
                    products.Remove(product);
                    products.Save();
                    page.Products.Remove(product);
                    if (page.Products.Count <= 0)
                    {
                        pages.Remove(page);
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
        [Area("UsersControl")]
        public IActionResult Users()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Users", PermissionType.View)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IUsers users = new UsersRepository(db);
                    return View(users.GetAll().ToList());
                }
            }
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Users", PermissionType.View);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
            }
        }
        [HttpPost]
        public IActionResult UpdateUsers()
        {
            return RedirectToRoute("/Admin/UsersControl/Users");
        }
        [Area("OrdersControl")]
        public IActionResult Orders()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Orders", PermissionType.View)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IOrders orders = new OrdersRepository(db);
                    IEnumerable<Order> orderModel = orders.GetAllWithInclude(o => o.User).Include(o => o.Product).ToList();
                    return View(orderModel);
                }
            }
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Orders", PermissionType.View);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
            }
        }
        [HttpPost]
        public IActionResult UpdateOrders()
        {
            return RedirectToRoute("/Admin/OrdersControl/Orders");
        }
        [Area("OrdersControl")]
        public IActionResult RecallOrder()
        {
            if (authorizationService.AuthorizeAsync(User, RolesManagerService.GeneratePermissionNameForModel("Orders", PermissionType.Delete)).Result.Succeeded)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IOrders orders = new OrdersRepository(db);
                    Order orderModel = orders.GetAllWithInclude(o => o.User).Include(o => o.Product).FirstOrDefault();
                    return View(orderModel);
                }
            }
            else
            {
                string permission = RolesManagerService.GeneratePermissionNameForModel("Orders", PermissionType.Delete);
                logger.LogInformation($"У пользователя {User.Identity.Name} нет разрешения {permission} для доступа к ресурсу.");
                return RedirectToAction("NoPermissions", new { permission });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RecallOrder(string id, string reason)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IOrders orders = new OrdersRepository(db);
                Order order = orders.GetAllWithInclude(o => o.User).Where(o => o.OrderId == new Guid(id)).FirstOrDefault();
                order.OrderStatus = OrderStatus.CLOSED;
                await orders.SaveAsync();
                logger.LogInformation($"Заказ {order.OrderId} пользователя {order.User.Email} открытый в {order.OrderDate} " +
                    $"был закрыт пользователем {User.Identity.Name}. \nПричина: {reason}");
                return RedirectToRoute("/Admin/OrdersControl/RecallOrder");
            }
        }
        [Area("OrdersControl")]
        public IActionResult OrderDetails(string orderid)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IOrders orders = new OrdersRepository(db);
                var guid = new Guid(orderid);
                return View(orders.GetAllWithInclude(o => o.User).Include(o => o.Product).Where(u => u.OrderId == guid).ToList().FirstOrDefault());
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        [Area("RolesControl")]
        public IActionResult UserRoles(int userId)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                IUsers users = new UsersRepository(db);
                User user = users.GetByID(userId);
                List<UserRolesViewModel> userRoles = new List<UserRolesViewModel>();
                foreach(var role in roles.GetAll().ToList())
                {
                    bool isInRole = user.Roles.Any(u => u.Name == role.Name);
                    userRoles.Add(new UserRolesViewModel { RoleName = role.Name, Selected = isInRole });
                }
                ManageUserRolesViewModel manageUserRolesViewModel = new ManageUserRolesViewModel { RolesVM = userRoles, UserName = user.Name };
                return View(manageUserRolesViewModel);
            }
        }
        [Area("RolesControl")]
        [HttpPost]
        public IActionResult UserRoles(ManageUserRolesViewModel manageUserRolesViewModel)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                IUsers users = new UsersRepository(db);
                User user = users.Find(u => u.Name == manageUserRolesViewModel.UserName).FirstOrDefault();
                user.Roles.Clear();

                foreach (var role in manageUserRolesViewModel.RolesVM)
                {
                    if (role.Selected)
                    {
                        user.Roles.Add(new Role { Name = role.RoleName });
                    }
                }
                return RedirectToAction("UserRoles", "Admin", new { userId = user.Id });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        [Area("RolesControl")]
        public IActionResult Roles()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                return View(roles.GetAll().ToList());
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        [Area("RolesControl")]
        public IActionResult CreateRole()
        {
            return View();
        }
        [Area("RolesControl")]
        [HttpPost]
        public IActionResult CreateRole(string roleName)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                Role role = new Role
                {
                    Name = roleName
                };
                roles.Add(role);
                roles.Save();
                return RedirectToAction("Roles", "Admin");
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        [Area("RolesControl")]
        public IActionResult EditRole(int roleId)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                Role role = roles.GetByID(roleId);
                return View(role);
            }
        }
        [Area("RolesControl")]
        [HttpPost]
        public IActionResult EditRole(string roleName)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                Role role = roles.Find(r => r.Name == roleName).FirstOrDefault();
                role.Name = roleName;
                roles.Save();
                return RedirectToAction("Roles", "Admin");
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        [Area("RolesControl")]
        public IActionResult DeleteRole(int roleId)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                Role role = roles.GetByID(roleId);
                return View(role.Name);
            }
        }
        [Area("RolesControl")]
        [HttpPost]
        public IActionResult DeleteRole(string roleName)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                Role role = roles.Find(r => r.Name == roleName).FirstOrDefault();
                roles.Remove(role);
                roles.Save();
                return RedirectToAction("Roles", "Admin");
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin")]
        [Area("PermissionsControl")]
        public IActionResult RolePermissions(int roleId)
        {
            using(var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                IRoles roles = new RolesRepository(db);
                Role role = roles.GetAllWithInclude(r => r.Permissions).Where(r => r.Id == roleId).FirstOrDefault();
                PermissionViewModel permissionViewModel = new PermissionViewModel();
                permissionViewModel.RoleName = role.Name;
                foreach(var permission in permissions.GetAll().ToList())
                {
                    bool hasPermission = role.Permissions.Any(r => r.Id == permission.Id);
                    permissionViewModel.RoleClaims.Add(new RoleClaimViewModel { Type = typeof(Permission).Name, Value = permission.PermissionName, Selected = hasPermission });
                }
                return View(permissionViewModel);
            }
        }
        [Area("PermissionsControl")]
        [HttpPost]
        public IActionResult RolePermissions(PermissionViewModel permissionViewModel)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                IRoles roles = new RolesRepository(db);
                Role role = roles.GetAllWithInclude(r => r.Permissions).Where(r => r.Name == permissionViewModel.RoleName).FirstOrDefault();
                role.Permissions.Clear();
                foreach (var permission in permissionViewModel.RoleClaims)
                {
                    if (permission.Selected)
                    {
                        role.Permissions.Add(permissions.Find(p => p.PermissionName == permission.Value).FirstOrDefault());
                    }
                }
                return RedirectToAction("RolePermissions", "Admin", new { roleId = role.Id });
            }
        }
    }
}
