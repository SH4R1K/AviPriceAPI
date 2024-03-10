using AviPriceUI.Data;
using AviPriceUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AviPriceUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AviApiContext _context;

        public CategoriesController(AviApiContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var rootCategory = await _context.Categories.FirstOrDefaultAsync();
            List<CategoryTreeNode> categoryTreeNodes = await CategoryToCategoryTreeNodesAsync(new List<Category> { rootCategory });
            return View(categoryTreeNodes);
        }

        private async Task<List<CategoryTreeNode>> CategoryToCategoryTreeNodesAsync(List<Category> categories)
        {
            List<CategoryTreeNode> categoryTreeNodes = new List<CategoryTreeNode>();
            foreach (var category in categories)
            {
                var categoryTreeNode = new CategoryTreeNode
                {
                    Id = category.IdCategory,
                    Name = category.Name,
                    Category = category
                };
                categoryTreeNode.Children = await CategoryToCategoryTreeNodesAsync(await _context.Categories.Where(c => c.IdParentCategory == category.IdCategory).ToListAsync());
                categoryTreeNodes.Add(categoryTreeNode);
            }
            return categoryTreeNodes;
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.IdParentCategoryNavigation)
                .FirstOrDefaultAsync(m => m.IdCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["IdParentCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategory,Name,IdParentCategory")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdParentCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", category.IdParentCategory);
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["IdParentCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", category.IdParentCategory);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategory,Name,IdParentCategory")] Category category)
        {
            if (id != category.IdCategory)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.IdCategory))
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
            ViewData["IdParentCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", category.IdParentCategory);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.IdParentCategoryNavigation)
                .FirstOrDefaultAsync(m => m.IdCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.IdCategory == id);
        }
    }
}
