using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.ViewModels;
using System.Data;

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
        // GET: Roles
        public async Task<IActionResult> Roles()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                return roles.GetAll() != null ?
                            View(await roles.GetAllAsync()) :
                            Problem("Entity set 'ApplicationDbContext.Roles'  is null.");
            }
        }

        // GET: Roles/Create
        public IActionResult CreateRole()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole([Bind("Id,Name")] Role role)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IRoles roles = new RolesRepository(db);
                    roles.Add(role);
                    await roles.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> EditRole(int? id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                if (id == null || roles.GetAll() == null)
                {
                    return NotFound();
                }

                var role = await roles.FindAsync(r => r.Id == id);
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(int id, [Bind("Id,Name")] Role role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new ApplicationDbContext(connectionString))
                    {
                        IRoles roles = new RolesRepository(db);
                        Role oldRole = roles.Find(r => r.Id == role.Id).FirstOrDefault();
                        oldRole = role;
                        await roles.SaveAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> DeleteRole(int? id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                if (id == null || roles.GetAll() == null)
                {
                    return NotFound();
                }

                var role = roles.Find(m => m.Id == id).FirstOrDefault();
                if (role == null)
                {
                    return NotFound();
                }

                return View(role);
            }
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleConfirmed(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                if (roles.GetAll() == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Roles'  is null.");
                }
                var role = (await roles.FindAsync(r => r.Id == id)).FirstOrDefault();
                if (role != null)
                {
                    roles.Remove(role);
                }

                await roles.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RoleExists(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IRoles roles = new RolesRepository(db);
                return roles.IfAny(e => e.Id == id);
            }
        }
        // GET: Permissions
        public async Task<IActionResult> Permissions()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                return permissions.GetAll() != null ?
                            View(await permissions.GetAllAsync()) :
                            Problem("Entity set 'ApplicationDbContext.Permissions'  is null.");
            }
        }

        // GET: Permissions/Create
        public IActionResult CreatePermission()
        {
            return View();
        }

        // POST: Permissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePermission([Bind("Id,PermissionName,IsGranted")] Permission permission)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IPermissions permissions = new PermissionsRepository(db);
                    permissions.Add(permission);
                    await permissions.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(permission);
        }

        // GET: Permissions/Edit/5
        public async Task<IActionResult> EditPermission(int? id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                if (id == null || permissions.GetAll() == null)
                {
                    return NotFound();
                }

                var permission = await permissions.FindAsync(p => p.Id == id);
                if (permission == null)
                {
                    return NotFound();
                }
                return View(permission);
            }
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPermission(int id, [Bind("Id,PermissionName,IsGranted")] Permission permission)
        {
            if (id != permission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new ApplicationDbContext(connectionString))
                    {
                        IPermissions permissions = new PermissionsRepository(db);
                        Permission oldPermission = permissions.Find(p => p.Id == permission.Id).FirstOrDefault();
                        oldPermission = permission;
                        await permissions.SaveAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionExists(permission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(permission);
        }

        // GET: Permissions/Delete/5
        public async Task<IActionResult> DeletePermission(int? id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                if (id == null || permissions.GetAll() == null)
                {
                    return NotFound();
                }

                var permission = (await permissions.FindAsync(p => p.Id == id))
                    .FirstOrDefault();
                if (permission == null)
                {
                    return NotFound();
                }

                return View(permission);
            }
        }

        // POST: Permissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermissionConfirmed(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                if (permissions.GetAll() == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Permissions'  is null.");
                }
                var permission = (await permissions.FindAsync(p => p.Id == id)).FirstOrDefault();
                if (permission != null)
                {
                    permissions.Remove(permission);
                }

                await permissions.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool PermissionExists(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IPermissions permissions = new PermissionsRepository(db);
                return permissions.IfAny(e => e.Id == id);
            }
        }
    }
}
