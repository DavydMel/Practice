using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models.Store;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings;
using System.Text;

namespace Shop.Controllers
{
    public class UserController : Controller
    {
        protected StoreDbContext storeDbContext;

        public UserController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password, bool KeepLoggedIn)
        {
            var mes = new List<string>();

            if (Email == null)
            {
                mes.Add("Email is required");
            }
            if (Password == null)
            {
                mes.Add("Password is required");
            }

            if (mes.Count > 0)
            {
                ViewBag.ValidateMessage = mes;
                return View();
            }

            var user = storeDbContext.User.FirstOrDefault(user => user.Email == Email);

            if (user == null)
            {
                mes.Add("Incorrect email or there is no account with this email");
                ViewBag.ValidateMessage = mes;
                return View();
            }

            if (user.Password != Convert.ToBase64String(Encoding.UTF8.GetBytes(Password)))
            {
                mes.Add("Incorrect password");
                ViewBag.ValidateMessage = mes;
                return View();
            }

            if (user.KeepLoggedIn != KeepLoggedIn)
            {
                user.KeepLoggedIn = KeepLoggedIn;
                storeDbContext.SaveChanges();
            }

            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = user.KeepLoggedIn
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string Name, string Email, string Password1, string Password2, bool KeepLoggedIn)
        {
            var mes = new List<string>();

            if (Name == null)
            {
                mes.Add("Name is rerquired");
            }
            if (Email == null || storeDbContext.User.FirstOrDefault(user => user.Name == Name) != null)
            {
                mes.Add("Email is incorrect or already in use");
            }
            if (Password1 == null || Password2 == null || Password1.Length < 8 || Password1 != Password2)
            {
                mes.Add("Password is too weak or does not match confirm");
            }

            if (mes.Count > 0)
            {
                ViewBag.ValidateMessage = mes;
                return View();
            }

            var user = new User();
            user.Name = Name;
            user.Email = Email;
            user.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password1));
            //string pas = Encoding.UTF8.GetString(Convert.FromBase64String(user.Password));
            user.KeepLoggedIn = KeepLoggedIn;

            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = user.KeepLoggedIn
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            storeDbContext.User.Add(user);
            storeDbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
