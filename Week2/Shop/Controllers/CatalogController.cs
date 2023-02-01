using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            ViewData["isLogged"] = claimUser.Identity.IsAuthenticated;
            return View();
        }

        public IActionResult Item()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            ViewData["isLogged"] = claimUser.Identity.IsAuthenticated;
            return View();
        }
    }
}