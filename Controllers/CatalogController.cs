using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Query;
using Silerium.Models.Repositories;
using Silerium.ViewModels;

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
        public IActionResult Products(string subcategory_name, string category_name, string? product_name, string sort_order,
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
                    case nameof(ViewModels.SortOrder.NAME_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.Name).ToList();
                        break;
                    case nameof(ViewModels.SortOrder.NAME_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.Name).ToList();
                        break;
                    case nameof(ViewModels.SortOrder.POP_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Include(p => p.Orders).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(ViewModels.SortOrder.POP_ASC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Include(p => p.Orders).Where(p => p.Page.Id == page)
                    .OrderBy(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(ViewModels.SortOrder.PRICE_DESC):
                        _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).Where(p => p.Page.Id == page)
                    .OrderByDescending(p => p.PriceRub).ToList();
                        break;
                    case nameof(ViewModels.SortOrder.PRICE_ASC):
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
                return View(new ProductsCatalogViewModel
                {
                    SubcategoryName = subcategory_name,
                    Products = _products,
                    Subcategories = subcategories.GetAll().ToList(),
                    Categories = categories.GetAll().ToList(),
                    //ProductsQuery = new ProductsQuery { Page = page, SortOrder = Enum.Parse<Models.Query.SortOrder>(sort_order) },
                    SortOrder = Enum.Parse<ViewModels.SortOrder>(sort_order),
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
    }
}
