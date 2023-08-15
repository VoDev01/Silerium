using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.Models.Interfaces;
using Silerium.Models.Repositories;
using Silerium.ViewModels;
using Silerium.ViewModels.AuthModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Dadata;
using Dadata.Model;

namespace Silerium.Controllers
{
    public class UserController : Controller
    {
        private readonly string connectionString;
        private readonly ILogger<UserController> logger;
        public UserController(ILogger<UserController> logger)
        {
            this.logger = logger;
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("Default");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult EditProfile()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                UserViewModel userViewModel = new UserViewModel();
                string userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                userViewModel.User = users.FindSetByCondition(u => u.Email == userEmail).FirstOrDefault();
                return View(userViewModel);
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult EditProfile(UserViewModel userVM, int country)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);

                User? user = userVM.User;
                user.Name = userVM.User.Name == string.Empty ? user.Name : userVM.User.Name;
                user.Surname = userVM.User.Surname == string.Empty ? user.Surname : userVM.User.Surname;
                user.Email = userVM.User.Email == string.Empty ? user.Email : userVM.User.Email;
                user.BirthDate = userVM.User.BirthDate;
                try
                {
                    byte[] imageData;
                    using (var stream = new BinaryReader(userVM.PfpFile.OpenReadStream())) //Retrieving image through FormFile Interface
                    {
                        imageData = stream.ReadBytes((int)userVM.PfpFile.Length);
                    }
                    user.ProfilePicture = imageData;
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return RedirectToAction("Profile", "User");
                }
                users.Save();
                return RedirectToAction("Profile", "User");
            }
        }
        [Route("User/Profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme)]
        //Get the profile page of the specified user
        public IActionResult Profile(string? returnUrl)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                if (returnUrl == null)
                {
                    try
                    {
                        IUsers users = new UsersRepository(db);
                        UserViewModel userVM = new UserViewModel();
                        User? user = null;
                        string userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                        user = users.FindSetByCondition(u => u.Email == userEmail).FirstOrDefault();
                        if (user != null)
                            userVM.User = user;
                        else
                        {
                            logger.LogError($"User with access token {HttpContext.Session.GetString("access_token")} was not authorized");
                            return Unauthorized();
                        }
                        return View(userVM);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.Message);
                        return RedirectToAction("Error", "Home");
                    }
                }
                else
                {
                    return LocalRedirect(returnUrl);
                }
            }
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> CheckEmail(string Email)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                if (await users.IfAnyAsync(u => u.Email == Email))
                {
                    return Json(false);
                }
                else
                    return Json(true);
            }
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> CheckPhone(string Phone)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                var api = new CleanClientAsync(Environment.GetEnvironmentVariable("DADATA_API_TOKEN"), Environment.GetEnvironmentVariable("DADATA_SECRET"));
                var phoneResult = await api.Clean<Phone>(Phone);
                if(phoneResult.qc_conflict == "2" || phoneResult.qc_conflict == "3")
                {
                    logger.LogWarning($"Город или регион адреса телефона {phoneResult.source} отличаются от указанных. " +
                        $"Город: {phoneResult.city}, регион: {phoneResult.region}.");
                }
                if(phoneResult.qc == "0" || phoneResult.qc == "7")
                {
                    return Json(true);
                }
                else if (phoneResult.qc == "2")
                {
                    logger.LogWarning($"Телефон {phoneResult.source} пустой или заведомо мусорный.");
                    return Json($"Введите действующий номер телефона.");
                }
                else if (phoneResult.qc == "1")
                {
                    return Json(false);
                }
                else if (phoneResult.qc == "3")
                {
                    logger.LogWarning($"Обнаружено несколько телефонов, распознан первый. Просьба проверить телефон {phoneResult.source} в ручную в случае несовпадений.");
                    return Json(true);
                }
                return Json(false);
            }
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> CheckPasswords(string Password, string ConfirmPassword)
        {
            if(Password == ConfirmPassword) 
            { 
                return Json(true); 
            }
            else
            {
                return Json(false);
            }
        }
        [Route("User/Login")]
        public IActionResult Login()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                UserViewModel userVM = new UserViewModel();
                UserLoginViewModel userLoginVM = new UserLoginViewModel
                {
                    ReturnUrl = "/User/Profile"
                };
                return View(userLoginVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/Login")]
        public async Task<IActionResult> Login(UserLoginViewModel userLoginVM, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IUsers users = new UsersRepository(db);
                    User? user = users.GetAllWithInclude(u => u.Orders).Where(u =>
                        u.Email == userLoginVM.Email &&
                        u.Password == userLoginVM.Password).FirstOrDefault();
                    if (user != null)
                    {
                        SecurityToken jwt;
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Role, user.Role)
                        };
                        ClaimsIdentity claimsIdentity;
                        ClaimsPrincipal claimsPrincipal;

                        if (userLoginVM.RememberMe) //Use cookies and jwt authentication
                        {
                            claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                            if (token != null)
                                jwt = new JwtSecurityTokenHandler().ReadToken(token);
                            else
                                jwt = new JwtSecurityToken(
                                issuer: JWTAuthOptions.ISSUER,
                                audience: JWTAuthOptions.AUDIENCE,
                                claims: claims,
                                expires: DateTime.UtcNow.AddMinutes(1),
                                signingCredentials: new SigningCredentials(JWTAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                                );
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                user.IsOnline = true;
                                users.Save();
                                return Redirect(returnUrl ?? "/");
                            }
                            else
                            {
                                logger.LogError($"An attempt of Open Redirect attack with {returnUrl} URL adress.");
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else //Use jwt authentication
                        {
                            claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                            string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                            if (token != null)
                                jwt = new JwtSecurityTokenHandler().ReadToken(token);
                            else
                                jwt = new JwtSecurityToken(
                                issuer: JWTAuthOptions.ISSUER,
                                audience: JWTAuthOptions.AUDIENCE,
                                claims: claims,
                                expires: DateTime.UtcNow.AddMinutes(1),
                                signingCredentials: new SigningCredentials(JWTAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                                );

                            if (Url.IsLocalUrl(returnUrl))
                            {
                                HttpContext.Session.SetString("access_token", new JwtSecurityTokenHandler().WriteToken(jwt));
                                user.IsOnline = true;
                                users.Save();
                                return Redirect(returnUrl ?? "/");
                            }
                            else
                            {
                                logger.LogError($"An attempt of Open Redirect attack with {returnUrl} URL adress.");
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                    else
                    {
                        if (users.FindSetByCondition(u => u.Password == userLoginVM.Password).Count() == 0)
                        {
                            TempData["Wrong Password"] = "Неверный пароль.";
                            return RedirectToAction("Login", "User", new { ReturnUrl = returnUrl });
                        }
                        else if (users.FindSetByCondition(u => u.Email == userLoginVM.Email).Count() == 0)
                        {
                            TempData["Wrong Email"] = "Неверный email.";
                            return RedirectToAction("Login", "User", new { ReturnUrl = returnUrl });
                        }
                        else
                        {
                            TempData["User not found"] = "Такого профиля не существует. Зарегистрируйтесь и создайте новый.";
                            return RedirectToAction("Login", "User", new { ReturnUrl = returnUrl });
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("ModelState Invalid", "Заполненные данные не верны.");
                return RedirectToAction("Login", "User");
            }
        }

        [Route("User/Register")]
        public IActionResult Register(string? returnUrl, string? error = null)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                if (error != null)
                    ModelState.AddModelError("ServerError", error);
                return View(new UserRegisterViewModel { ReturnUrl = returnUrl });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/Register")]
        public async Task<IActionResult> Register(UserRegisterViewModel userRegisterVM, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IUsers users = new UsersRepository(db);
                    var api = new SuggestClientAsync(Environment.GetEnvironmentVariable("DADATA_API_TOKEN"));
                    var apiResult = await api.Iplocate(Request.HttpContext.Connection.RemoteIpAddress.ToString());
                    if (!Request.Cookies.Any(c => c.Key == "UserCity"))
                    {
                        if(apiResult.location != null)
                            Response.Cookies.Append("UserCity", apiResult.location.value, new CookieOptions { Expires = DateTime.Now.AddHours(24) });
                    }
                    User user = new User
                    {
                        Name = userRegisterVM.Name,
                        Surname = userRegisterVM.Surname,
                        Password = userRegisterVM.Password,
                        Email = userRegisterVM.Email,
                        BirthDate = userRegisterVM.BirthDate,
                        Country = userRegisterVM.Country,
                        Phone = userRegisterVM.Phone,
                        HomeAdress = userRegisterVM.HomeAdress,
                        City = apiResult.location?.value,
                        Role = "Client"
                    };

                    byte[] imageData;
                    if (userRegisterVM.PfpFile != null)
                    {
                        using (var stream = new BinaryReader(userRegisterVM.PfpFile.OpenReadStream()))
                        {
                            imageData = stream.ReadBytes((int)userRegisterVM.PfpFile.Length);
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images") + "\\default_user.png";
                        imageData = System.IO.File.ReadAllBytes(path);
                    }
                    user.ProfilePicture = imageData;

                    users.Create(user);
                    users.Save();

                    return RedirectToAction("Login", "User", new { returnUrl });
                }
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });
                foreach (var error in errors)
                {
                    logger.LogError(error.Errors.FirstOrDefault().ErrorMessage);
                }
                return RedirectToAction("Register", "User", new { error = "Заполненные данные не верны." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout(int id)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                users.GetByID(id).IsOnline = false;
                HttpContext.Session.Remove("access_token");
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                users.Save();
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ", " + CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult ShopCart(string order_status = "ISSUING")
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                IOrders orders = new OrdersRepository(db);
                string? userEmail = HttpContext.User.Identity.Name;
                if (userEmail != null)
                {
                    User user = users.FindSetByCondition(u => u.Email == userEmail).FirstOrDefault();
                    var orderStatusVal = Enum.Parse(typeof(OrderStatus), order_status);
                    UserViewModel userVM = new UserViewModel
                    {
                        User = user,
                        UserOrders = orders
                        .GetAllWithInclude(o => o.Product)
                        .ThenInclude(p => ((Product)p).Images)
                        .Where(o => o.UserId == user.Id && o.OrderStatus.Equals(orderStatusVal)).ToList()
                    };
                    return View(userVM);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }
        public IActionResult ShopCartFilter(string order_status)
        {
            return RedirectToAction("ShopCart", "User", new { order_status });
        }
        public IActionResult CheckoutOrder()
        {
            return View();
        }
        public IActionResult EditOrder()
        {
            return View();
        }
        public IActionResult DeleteOrder()
        {
            return View();
        }
    }
}