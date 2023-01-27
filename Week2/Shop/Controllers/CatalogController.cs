using Microsoft.AspNetCore.Mvc;

namespace Shop.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Item()
        {
            return View();
        }
    }
}