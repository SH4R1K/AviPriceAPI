using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AviPriceUI.Data;
using AviPriceUI.Models;

namespace AviPriceUI.Controllers
{
    public class CellMatricesController : Controller
    {
        private readonly AviApiContext _context;

        public CellMatricesController(AviApiContext context)
        {
            _context = context;
        }

        // GET: CellMatrices
        public async Task<IActionResult> Index(int id)
        {
            var aviApiContext = _context.CellMatrices
                .Include(c => c.IdCategoryNavigation)
                .Include(c => c.IdLocationNavigation)
                .Where(cm => cm.IdMatrixNavigation.IdMatrix == id);
            return View(await aviApiContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<CellMatrix> cellMatrices)
        {
            _context.CellMatrices.UpdateRange(cellMatrices);
            _context.SaveChanges();
            return View(cellMatrices);
        }


        // GET: CellMatrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cellMatrix = await _context.CellMatrices
                .Include(c => c.IdCategoryNavigation)
                .Include(c => c.IdLocationNavigation)
                .Include(c => c.IdMatrixNavigation)
                .FirstOrDefaultAsync(m => m.IdCellMatrix == id);
            if (cellMatrix == null)
            {
                return NotFound();
            }

            return View(cellMatrix);
        }

        // GET: CellMatrices/Create
        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory");
            ViewData["IdLocation"] = new SelectList(_context.Locations, "IdLocation", "IdLocation");
            ViewData["IdMatrix"] = new SelectList(_context.Matrices, "IdMatrix", "IdMatrix");
            return View();
        }

        // POST: CellMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCellMatrix,Price,IdLocation,IdCategory,IdMatrix")] CellMatrix cellMatrix)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cellMatrix);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", cellMatrix.IdCategory);
            ViewData["IdLocation"] = new SelectList(_context.Locations, "IdLocation", "IdLocation", cellMatrix.IdLocation);
            ViewData["IdMatrix"] = new SelectList(_context.Matrices, "IdMatrix", "IdMatrix", cellMatrix.IdMatrix);
            return View(cellMatrix);
        }

        // GET: CellMatrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cellMatrix = await _context.CellMatrices.FindAsync(id);
            if (cellMatrix == null)
            {
                return NotFound();
            }
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", cellMatrix.IdCategory);
            ViewData["IdLocation"] = new SelectList(_context.Locations, "IdLocation", "IdLocation", cellMatrix.IdLocation);
            ViewData["IdMatrix"] = new SelectList(_context.Matrices, "IdMatrix", "IdMatrix", cellMatrix.IdMatrix);
            return View(cellMatrix);
        }

        // POST: CellMatrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCellMatrix,Price,IdLocation,IdCategory,IdMatrix")] CellMatrix cellMatrix)
        {
            if (id != cellMatrix.IdCellMatrix)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cellMatrix);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CellMatrixExists(cellMatrix.IdCellMatrix))
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
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "IdCategory", cellMatrix.IdCategory);
            ViewData["IdLocation"] = new SelectList(_context.Locations, "IdLocation", "IdLocation", cellMatrix.IdLocation);
            ViewData["IdMatrix"] = new SelectList(_context.Matrices, "IdMatrix", "IdMatrix", cellMatrix.IdMatrix);
            return View(cellMatrix);
        }

        // GET: CellMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cellMatrix = await _context.CellMatrices
                .Include(c => c.IdCategoryNavigation)
                .Include(c => c.IdLocationNavigation)
                .Include(c => c.IdMatrixNavigation)
                .FirstOrDefaultAsync(m => m.IdCellMatrix == id);
            if (cellMatrix == null)
            {
                return NotFound();
            }

            return View(cellMatrix);
        }

        // POST: CellMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cellMatrix = await _context.CellMatrices.FindAsync(id);
            if (cellMatrix != null)
            {
                _context.CellMatrices.Remove(cellMatrix);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CellMatrixExists(int id)
        {
            return _context.CellMatrices.Any(e => e.IdCellMatrix == id);
        }
    }
}
