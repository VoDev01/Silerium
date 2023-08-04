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
using Microsoft.AspNetCore.Http.Extensions;

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
        [Authorize]
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
        [Authorize]
        //Get the profile page of the specified user
        public IActionResult Profile(string? t)
        {
            using (var db = new ApplicationDbContext(connectionString))
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
                        logger.LogError("User was not authenticated");
                        return RedirectToAction("Error", "Home");
                    }
                    return View(userVM);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return RedirectToAction("Error", "Home");
                }
            }
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> CheckEmail(string email)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                if (await users.IfAnyAsync(u => u.Email == email))
                {
                    return Json(false);
                }
                else
                    return Json(true);
            }
        }
        [Route("User/Login")]
        public IActionResult Login(string? t = null)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                UserViewModel userVM = new UserViewModel();

                if (t == null)
                {
                    UserLoginViewModel userLoginVM = new UserLoginViewModel
                    {
                        ReturnUrl = "/User/Profile" //HttpContext.Request.Query.Where(q => q.Key == "ReturnUrl").Select(q => q.Value).First()
                    };
                    return View(userLoginVM);
                }
                else
                    return RedirectToAction("Profile", new { t });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/Login")]
        public async Task<IActionResult> Login(UserLoginViewModel userLoginVM, string? t, string? returnUrl, bool remember)
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
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Role, "Client")
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        if (remember) //Use cookies authorization
                        {
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl ?? "/");
                            }
                            else
                            {
                                logger.LogError($"An attempt of Open Redirect attack with {returnUrl} URL adress.");
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else //Use jwt-tokens
                        {
                            claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                            claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            SecurityToken jwt;
                            if (t != null)
                                jwt = new JwtSecurityTokenHandler().ReadToken(t);
                            else
                                jwt = new JwtSecurityToken(
                                issuer: JWTAuthOptions.ISSUER,
                                audience: JWTAuthOptions.AUDIENCE,
                                claims: claims,
                                expires: DateTime.UtcNow.AddMinutes(1),
                                signingCredentials: new SigningCredentials(JWTAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                                );

                            QueryBuilder queryBuilder = new QueryBuilder
                            {
                                { "t", new JwtSecurityTokenHandler().WriteToken(jwt) }
                            };
                            UriBuilder uriBuilder = new UriBuilder { Path = returnUrl, Query = queryBuilder.ToString() };
                            string localUrl = uriBuilder.Uri.PathAndQuery.Substring(uriBuilder.Uri.PathAndQuery.IndexOf("/User/"));

                            if (Url.IsLocalUrl(localUrl))
                            {
                                return Redirect(localUrl ?? "/");
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
                            TempData["Wrong Password"] = "Неверный пароль";
                            return RedirectToAction("Login", "User", new { ReturnUrl = returnUrl });
                        }
                        else if (users.FindSetByCondition(u => u.Email == userLoginVM.Email).Count() == 0)
                        {
                            TempData["Wrong Email"] = "Неверный email";
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
                ModelState.AddModelError("ModelState Invalid", "Заполненные данные не верны");
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
        public async Task<IActionResult> Register(UserRegisterViewModel userRegisterVM, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (userRegisterVM.Password == userRegisterVM.ConfirmPassword)
                {
                    using (var db = new ApplicationDbContext(connectionString))
                    {
                        IUsers users = new UsersRepository(db);
                        User user = new User
                        {
                            Name = userRegisterVM.Name,
                            Surname = userRegisterVM.Surname,
                            Password = userRegisterVM.Password,
                            Email = userRegisterVM.Email,
                            BirthDate = userRegisterVM.BirthDate,
                            Country = userRegisterVM.Country,
                            Phone = userRegisterVM.Phone,
                            HomeAdress = userRegisterVM.HomeAdress
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

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Role, "Client")
                        };

                        var jwt = new JwtSecurityToken(
                                issuer: JWTAuthOptions.ISSUER,
                                audience: JWTAuthOptions.AUDIENCE,
                                claims: claims,
                                expires: DateTime.UtcNow.AddMinutes(1),
                                signingCredentials: new SigningCredentials(JWTAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                                );
                        QueryBuilder queryBuilder = new QueryBuilder
                        {
                            { "t", new JwtSecurityTokenHandler().WriteToken(jwt) }
                        };
                        UriBuilder uriBuilder = new UriBuilder { Path = returnUrl, Query = queryBuilder.ToString() };
                        string localUrl = uriBuilder.Uri.PathAndQuery.Substring(uriBuilder.Uri.PathAndQuery.IndexOf("/User/"));
                        if (Url.IsLocalUrl(localUrl))
                        {
                            
                            return Redirect(localUrl ?? "/");
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
                    return RedirectToAction("Register", "User", new { error = "Пароли должны совпадать" });
                }
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });
                foreach (var error in errors)
                {
                    logger.LogError(error.Errors.FirstOrDefault().ErrorMessage);
                }
                return RedirectToAction("Register", "User", new { error = "Заполненные данные не верны" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }
    }
}