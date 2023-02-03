using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models.Store;

namespace Shop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly StoreDbContext _context;

        public CategoryController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(await _context.Category.ToListAsync());
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var prefix = @"wwwroot\img\category";
                var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
                var filePath = Path.Combine(prefix, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                category.ImgName = fileName;
            }
            else
            {
                return View(category);
            }

            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImgName")] Category category, IFormFile file)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            try
            {
                if (file != null && file.Length > 0)
                {
                    var prefix = @"wwwroot\img\category";
                    var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
                    var filePath = Path.Combine(prefix, fileName);

                    var oldImg = Path.Combine(prefix, category.ImgName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                        if (System.IO.File.Exists(oldImg))
                        {
                            System.IO.File.Delete(oldImg);
                        }
                    }

                    category.ImgName = fileName;
                }

                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'StoreDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);

                var prefix = @"wwwroot\img\category";
                var oldImg = Path.Combine(prefix, category.ImgName);


                if (System.IO.File.Exists(oldImg))
                {
                    System.IO.File.Delete(oldImg);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Products(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewBag.Category = category.Name;

            var products = _context.Product.Where(p => p.CategoryId == id);

            return View(products);
        }

        private bool CategoryExists(int id)
        {
           return _context.Category.Any(e => e.Id == id);
        }
    }
}
