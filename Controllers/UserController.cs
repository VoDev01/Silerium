using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Silerium.Data;
using Silerium.Models;
using Silerium.ViewModels;

namespace Silerium.Controllers
{
    public class UserController : Controller
    {
        public static User? CurrentUser { get; private set; } = null;
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
        public IActionResult EditProfile()
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                UserViewModel userViewModel = new UserViewModel();
                userViewModel.User = CurrentUser;
                return View(userViewModel);
            }
        }
        [HttpPost]
        public IActionResult EditProfile(UserViewModel userVM, int country)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);

                User user = users.GetByID(CurrentUser.Id);
                CurrentUser.NickName = userVM.User.NickName == string.Empty ? CurrentUser.NickName : userVM.User.NickName;
                CurrentUser.Name = userVM.User.Name == string.Empty ? CurrentUser.Name : userVM.User.Name;
                CurrentUser.Surname = userVM.User.Surname == string.Empty ? CurrentUser.Surname : userVM.User.Surname;
                CurrentUser.Email = userVM.User.Email == string.Empty ? CurrentUser.Email : userVM.User.Email;
                CurrentUser.BirthDate = userVM.User.BirthDate;
                try
                {
                    byte[] imageData;
                    using (var stream = new BinaryReader(userVM.PfpFile.OpenReadStream())) //Retrieving image through FormFile Interface
                    {
                        imageData = stream.ReadBytes((int)userVM.PfpFile.Length);
                    }
                    CurrentUser.ProfilePicture = imageData;
                    user = CurrentUser;
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
        public async Task<IActionResult> SetMovieRating(int rating, int movieid)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                try
                {
                    User? user = users
                        .GetAll().ToList().FirstOrDefault();
                    await users.SaveAsync();
                    return RedirectToAction("Page", "Movies", new { id = movieid });
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    return RedirectToAction("Page", "Movies", new { id = movieid });
                }
            }
        }
        [Route("User/Profile")]
        [Route("User/Profile/{userUrlId}")]
        //Get the profile page of the specified user
        public IActionResult Profile(string? userUrlId = null) //UserUlrId is an id specified by the user for an easier identification
        {                                                        //UserId is used in the table as a key and also can be used to identificate user if UserUrlId is not given
            using (var db = new ApplicationDbContext(connectionString))
            {
                try
                {
                    IUsers users = new UsersRepository(db);
                    UserViewModel userVM = new UserViewModel();
                    User? user = null;
                    if (CurrentUser == null)
                    {
                        user = users.GetAllWithInclude(u => u.Country).Include(u => u.MoviesRatings).Where(u => u.UserUrlId == userUrlId).FirstOrDefault();
                        if (user != null)
                            userVM.User = user;
                        else
                            return RedirectToAction("Error", "Home");
                    }
                    else
                        userVM.User = CurrentUser;
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
        [AcceptVerbs("Get", "Post")]
        public async Task<JsonResult> CheckNickName(string nickname)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                if (await users.IfAnyAsync(u => u.NickName == nickname))
                {
                    return Json(false);
                }
                else
                    return Json(true);
            }
        }
        [Route("User/Login")]
        public IActionResult Login(int? movieid = null)
        {
            if (CurrentUser == null)
            {
                if (movieid == null)
                    return View();
                else
                {
                    UserLoginViewModel userLoginVM = new UserLoginViewModel();
                    return View(userLoginVM);
                }
            }
            else
                return RedirectToAction("Profile");
        }

        [HttpPost]
        [Route("User/Login")]
        public IActionResult Login(UserLoginViewModel userLoginVM)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                IUsers users = new UsersRepository(db);
                User? user = users.GetAllWithInclude(u => u.Country).
                    Include(u => u.MoviesRatings).
                    ThenInclude(r => r.Movie).
                    Where(u => u.NickName == userLoginVM.Login
                    && u.Password == userLoginVM.Password
                    && u.Email == userLoginVM.Email).FirstOrDefault();
                if (user != null)
                {
                    CurrentUser = user;
                    if (userLoginVM.MovieId == null)
                        return RedirectToAction("Profile", new { user.UserUrlId });
                    else
                        return RedirectToAction("Page", "Movies", new { id = userLoginVM.MovieId });
                }
                else
                {
                    if (users.FindSetByCondition(u => u.Password == userLoginVM.Password).Count() == 0)
                    {
                        ModelState.AddModelError("Wrong Password", "Неверный пароль");
                        return View();
                    }
                    else if (users.FindSetByCondition(u => u.Email == userLoginVM.Email).Count() == 0)
                    {
                        ModelState.AddModelError("Wrong Email", "Неверный email");
                        return View();
                    }
                    else if (users.FindSetByCondition(u => u.NickName == userLoginVM.Login).Count() == 0)
                    {
                        ModelState.AddModelError("Wrong Login", "Неверный логин");
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("User not found", "Такого профиля не существует. Зарегистрируйтесь и создайте новый.");
                        logger.LogWarning($"User with nickname {userLoginVM.Login} was not found or wrong login data was typed");
                        return View();
                    }
                }
            }
        }

        [Route("User/Register")]
        public IActionResult Register(string? error = null)
        {
            using (var db = new ApplicationDbContext(connectionString))
            {
                UserViewModel userViewModel = new UserViewModel();
                if (error != null)
                    ModelState.AddModelError("ServerError", error);
                return View(userViewModel);
            }
        }

        [HttpPost]
        [Route("User/Register")]
        public IActionResult Register(UserViewModel userVM, string password_repeat, int country)
        {
            if (password_repeat == userVM.User.Password)
            {
                //if (ModelState.IsValid)
                //{
                using (var db = new ApplicationDbContext(connectionString))
                {
                    IUsers users = new UsersRepository(db);
                    UserViewModel userVMloc = new UserViewModel();
                    User user = new User
                    {
                        NickName = userVM.User.NickName,
                        Name = userVM.User.Name,
                        Surname = userVM.User.Surname,
                        Password = userVM.User.Password,
                        Email = userVM.User.Email,
                        BirthDate = userVM.User.BirthDate
                    };

                    byte[] imageData;
                    if (userVM.PfpFile != null)
                    {
                        using (var stream = new BinaryReader(userVM.PfpFile.OpenReadStream()))
                        {
                            imageData = stream.ReadBytes((int)userVM.PfpFile.Length);
                        }
                    }
                    else
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images") + "\\default_user.png";
                        imageData = System.IO.File.ReadAllBytes(path);
                    }
                    user.ProfilePicture = imageData;

                    userVMloc.User = user;
                    CurrentUser = user;
                    users.Create(user);
                    users.Save();

                    return RedirectToAction("Profile", "User", new { user.UserUrlId });
                }
                //}
                //else
                //{
                //    return RedirectToAction("Register", "User", new { error = "Заполненные данные не верны" });
                //}
            }
            else
            {
                return RedirectToAction("Register", "User", new { error = "Пароли должны совпадать" });
            }
        }
    }
}