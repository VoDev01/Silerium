using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.DTO;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silerium.Controllers
{
    public class CatalogController : Controller
    {
        private string connectionString;
        public ILogger<CatalogController> logger;
        public static readonly int productsAtPage = 20;
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
        public async Task<IActionResult> Products(string subcategory_name, string category_name, string? product_name, string sort_order,
            bool available, bool filter = false, int page = 1)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                ICategories categories = new CategoriesRepository(db);
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
                List<Product> _products = new List<Product>();
                switch (sort_order)
                {
                    case nameof(SortOrder.NAME_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.Name).ToList();
                        break;
                    case nameof(SortOrder.NAME_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.Name).ToList();
                        break;
                    case nameof(SortOrder.POP_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Include(p => p.Orders).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(SortOrder.POP_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Include(p => p.Orders).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(SortOrder.PRICE_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.PriceRub).ToList();
                        break;
                    case nameof(SortOrder.PRICE_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.PriceRub).ToList();
                        break;
                }
                if (filter)
                {
                    if (product_name != null)
                    {
                        if (products.IfAny(p => p.Name == product_name))
                        {
                            _products = _products.Where(p => p.Name.StartsWith(product_name)).ToList();
                        }
                    }
                    else if (category_name != "all" && subcategory_name != "all")
                    {
                        _products = _products.Where(p =>
                        p.Subcategory.Category.Name.ToLower() == category_name
                        && p.Subcategory.Name.ToLower() == category_name).ToList();
                    }
                    if (available)
                    {
                        _products = _products.Where(p => p.Available == true).ToList();
                    }
                }
                if (productsAtPage < _products.Count)
                    _products = _products.GetRange(productsAtPage * (page - 1), productsAtPage);
                HttpContext.Session.SetString("ProductsFilters", JsonSerializer.Serialize(
                    new ProductsFiltersDTO
                    {
                        CategoryName = category_name,
                        SubcategoryName = subcategory_name,
                        SortOrder = sort_order,
                        CurrentPage = page
                    }));
                return View(new ProductsCatalogViewModel
                {
                    SubcategoryName = subcategory_name,
                    Products = _products,
                    Subcategories = subcategories.GetAll().ToList(),
                    Categories = categories.GetAll().ToList(),
                    //ProductsQuery = new ProductsQuery { Page = page, SortOrder = Enum.Parse<Models.Query.SortOrder>(sort_order) },
                    SortOrder = Enum.Parse<SortOrder>(sort_order),
                    FirstPaginationIndex = firstPageIndex,
                    LastPaginationIndex = lastPageIndex,
                    CurrentPage = page
                });
            }
        }
        [HttpPost]
        public IActionResult Products(string? product_name, string category_name, string subcategory_name, string sort_order, bool available, int page)
        {
            return RedirectToAction("Products", "Catalog", new
            {
                product_name,
                category_name,
                subcategory_name,
                sort_order,
                available,
                filter = true,
                page
            });
        }
        public IActionResult AddToCart(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IProducts products = new ProductsRepository(db);
                ProductViewModel productVM = new ProductViewModel 
                { 
                    Product = products.GetAllWithInclude(p => p.Images).Include(p => p.Subcategory).Where(p => p.Id == id).FirstOrDefault()
                };
                return View(productVM);
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult AddToCart(int id, int amount) 
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IProducts products = new ProductsRepository(db);
                IOrders orders = new OrdersRepository(db); 
                IUsers users = new UsersRepository(db);
                string userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                User? user = users.FindSetByCondition(u => u.Email == userEmail).FirstOrDefault();
                if (user != null)
                {
                    Product product = products.GetByID(id - 1);
                    orders.Create(new Order
                    {
                        Product = product,
                        OrderAmount = amount,
                        OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        TotalPrice = amount * product.PriceRub,
                        OrderStatus = OrderStatus.ISSUING,
                        User = user
                    });
                    orders.Save();
                    ProductsFiltersDTO? productsFiltersDTO = 
                        JsonSerializer.Deserialize(HttpContext.Session.GetString("ProductsFilters") ?? "", typeof(ProductsFiltersDTO)) 
                        as ProductsFiltersDTO;
                    if (productsFiltersDTO != null)
                    {
                        return RedirectToAction("Products", "Catalog", new
                        {
                            category_name = productsFiltersDTO.CategoryName,
                            subcategory_name = productsFiltersDTO.SubcategoryName,
                            sort_order = productsFiltersDTO.SortOrder,
                            page = productsFiltersDTO.CurrentPage
                        });
                    }
                    else
                    {
                        logger.LogError("Error getting products filters at add to cart page.");
                        return RedirectToAction("Index", "Categories");
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
        }
    }
}
