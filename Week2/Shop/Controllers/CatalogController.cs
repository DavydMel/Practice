using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using System.Security.Claims;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        protected readonly StoreDbContext storeDbContext;

        public CatalogController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            ViewData["isLogged"] = claimUser.Identity.IsAuthenticated;
            return View(await storeDbContext.Product.ToListAsync());
        }
    }
}