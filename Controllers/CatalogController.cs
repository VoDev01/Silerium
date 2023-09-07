﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.ViewModels.ProductsModels;
using System.Security.Claims;
using System.Text.Json;

namespace Silerium.Controllers
{
    public class CatalogController : Controller
    {
        private string connectionString;
        public ILogger<CatalogController> logger;
        public static readonly int productsAtPage = 20;
        public static int CurrentLastPageIndex { get; set; } = 1;
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
            bool available, bool search = false, int page = 1)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                ISubcategories subcategories = new SubcategoriesRepository(db);
                ICategories categories = new CategoriesRepository(db);
                IProducts products = new ProductsRepository(db);

                List<Product> _products = products.GetAllWithInclude(p => p.Images).Include(p => p.Specifications).ToList();
                if (search)
                {
                    if (available)
                    {
                        if (product_name != null)
                        {
                            _products = _products.Where(p => p.Name.Contains(product_name) && p.Available == !available).ToList();
                        }
                        else if (category_name != "all" && subcategory_name != "all" && product_name == null)
                        {
                            _products = _products.Where(p =>
                            p.Subcategory.Category.Name.ToLower().Contains(category_name)
                            && p.Subcategory.Name.ToLower().Contains(category_name)
                            && p.Available == !available).ToList();
                        }
                        else if (category_name != "all" && subcategory_name != "all" && product_name != null)
                        {
                            _products = _products.Where(p =>
                            p.Subcategory.Category.Name.ToLower().Contains(category_name)
                            && p.Subcategory.Name.ToLower().Contains(category_name)
                            && p.Name.Contains(product_name)
                            && p.Available == !available).ToList();
                        }
                    }
                    else
                    {
                        if (product_name != null)
                        {
                            if (products.IfAny(p => p.Name.Contains(product_name)))
                            {
                                _products = _products.Where(p => p.Name.Contains(product_name) && p.Available == !available).ToList();
                            }
                        }
                        else if (category_name != "all" && subcategory_name != "all" && product_name == null)
                        {
                            _products = _products.Where(p =>
                            p.Subcategory.Category.Name.ToLower().Contains(category_name)
                            && p.Subcategory.Name.ToLower().Contains(category_name)
                            && p.Available == !available).ToList();
                        }
                        else if (category_name != "all" && subcategory_name != "all" && product_name != null)
                        {
                            _products = _products.Where(p =>
                            p.Subcategory.Category.Name.ToLower().Contains(category_name)
                            && p.Subcategory.Name.ToLower().Contains(category_name)
                            && p.Name.Contains(product_name)
                            && p.Available == !available).ToList();
                        }
                    }
                }
                switch (sort_order)
                {
                    case nameof(SortOrder.NAME_DESC):
                        _products = _products.OrderByDescending(p => p.Name).ToList();
                        break;
                    case nameof(SortOrder.NAME_ASC):
                        _products = _products.OrderBy(p => p.Name).ToList();
                        break;
                    case nameof(SortOrder.POP_DESC):
                        _products = _products.OrderByDescending(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(SortOrder.POP_ASC):
                        _products = _products.OrderBy(p => p.Orders.Count()).ToList();
                        break;
                    case nameof(SortOrder.PRICE_DESC):
                        _products = _products.OrderByDescending(p => p.PriceRub).ToList();
                        break;
                    case nameof(SortOrder.PRICE_ASC):
                        _products = _products.OrderBy(p => p.PriceRub).ToList();
                        break;
                }
                if (productsAtPage < _products.Count)
                    _products = _products.GetRange(productsAtPage * (page - 1), productsAtPage);
                HttpContext.Session.SetString("ProductsFilters", JsonSerializer.Serialize(
                    new ProductsFiltersViewModel
                    {
                        CategoryName = category_name,
                        SubcategoryName = subcategory_name,
                        SortOrder = sort_order,
                        CurrentPage = page
                    }));
                PaginationModel paginationModel = new PaginationModel("Products", "Catalog");
                paginationModel.CurrentPage = page;
                return View("~/Views/Catalog/Products.cshtml", new ProductsCatalogViewModel
                {
                    SubcategoryName = subcategory_name,
                    Products = _products,
                    Subcategories = subcategories.GetAll().ToList(),
                    Categories = categories.GetAll().ToList(),
                    SortOrder = Enum.Parse<SortOrder>(sort_order),
                    PaginationModel = paginationModel
                });
            }
        }
        [HttpPost]
        public IActionResult ProductsFilter(string subcategory_name, string category_name, string? product_name, string sort_order,
            bool available = true, int page = 1)
        {
            return RedirectToAction("Products", "Catalog", new
            {
                product_name,
                category_name,
                subcategory_name,
                sort_order,
                available,
                search = true,
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
                string userEmail = HttpContext.User.FindFirstValue("Name");
                User? user = users.Find(u => u.Email == userEmail).FirstOrDefault();
                if (user != null)
                {
                    Product product = products.GetByID(id - 1);
                    orders.Add(new Order
                    {
                        Product = product,
                        OrderAmount = amount,
                        OrderDate = DateTime.UtcNow,
                        TotalPrice = amount * product.PriceRub,
                        OrderStatus = OrderStatus.ISSUING,
                        User = user
                    });
                    orders.Save();
                    ProductsFiltersViewModel? productsFiltersDTO =
                        JsonSerializer.Deserialize(HttpContext.Session.GetString("ProductsFilters") ?? "", typeof(ProductsFiltersViewModel))
                        as ProductsFiltersViewModel;
                    if (productsFiltersDTO != null)
                    {
                        logger.LogInformation($"User {user.Email} added product {product.Name} to cart in quantity of {amount} pcs.");
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
                        logger.LogError("Error getting products filters at \"AddToCart\" page.");
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
