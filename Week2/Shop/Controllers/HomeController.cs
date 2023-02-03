using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shop.Data;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly StoreDbContext storeDbContext;

        public HomeController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = storeDbContext.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user != null) {
                ViewBag.isAdmin = user.Access > 0;
            }
            else
            {
                ViewBag.isAdmin = false;
            }
            ViewData["isLogged"] = claimUser.Identity.IsAuthenticated;

            var categories = storeDbContext.Category.ToList();
            return View(categories);
        }

        public async Task<IActionResult> Basket()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            ViewData["isLogged"] = claimUser.Identity.IsAuthenticated;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}