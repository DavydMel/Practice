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
    public class ProductController : Controller
    {
        private readonly StoreDbContext _context;

        public ProductController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            var storeDbContext = _context.Product.Include(p => p.Category);
            return View(await storeDbContext.ToListAsync());
        }

        public async Task<IActionResult> View(int? id)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            ViewData["isLogged"] = claimUser.Identity.IsAuthenticated;

            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Product.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Color,Sizes,Description,CategoryId")] Product product, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var prefix = @"wwwroot\img\product";
                var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
                var filePath = Path.Combine(prefix, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                product.ImgName = fileName;
            }
            else
            {
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Color,Sizes,Description,CategoryId,ImgName")] Product product, IFormFile file)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            try
            {
                if (file != null && file.Length > 0)
                {
                    var prefix = @"wwwroot\img\product";
                    var fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg"); ;
                    var filePath = Path.Combine(prefix, fileName);

                    var oldImg = Path.Combine(prefix, product.ImgName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                        if (System.IO.File.Exists(oldImg))
                        {
                            System.IO.File.Delete(oldImg);
                        }
                    }

                    product.ImgName = fileName;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
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

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            var user = _context.User.FirstOrDefault(user => user.Email == claimUser.FindFirstValue(ClaimTypes.Email));
            if (user == null || user.Access < 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'StoreDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);

                var prefix = @"wwwroot\img\product";
                var oldImg = Path.Combine(prefix, product.ImgName);


                if (System.IO.File.Exists(oldImg))
                {
                    System.IO.File.Delete(oldImg);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.Product.Any(e => e.Id == id);
        }
    }
}
