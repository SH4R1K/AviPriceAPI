using AviPriceUI.Data;
using AviPriceUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AviPriceUI.Controllers
{
    public class CellMatricesController : Controller
    {
        private readonly AviContext _context;

        IMemoryCache cache;

        private List<CellMatrix>? _cells
        {
            get
            {
                return cache.Get("CellMatrixList") as List<CellMatrix>;
            }
            set
            {
                cache.Set("CellMatrixList", value, DateTimeOffset.Now.AddDays(7));
            }
        }

        private int? _idMatrix
        {
            get
            {
                return cache.Get("IdMatrix") as int?;
            }
            set
            {
                cache.Set("IdMatrix", value, DateTimeOffset.Now.AddDays(7));
            }
        }

        public CellMatricesController(AviContext context, IMemoryCache memoryCache)
        {
            _context = context;
            cache = memoryCache;
        }

        // GET: CellMatrices
        public async Task<IActionResult> Index(int id)
        {
            return await LoadData(id);
        }

        public async Task<IActionResult> IndexNotEdit(int id)
        {
            var cellMatrices = await _context.CellMatrices
                .Include(cm => cm.IdCategoryNavigation)
                .Include(cm => cm.IdLocationNavigation)
                .Where(cm => cm.IdMatrix == id).ToListAsync();
            return View(new CellMatricesViewModel
            {
                CellMatrices = cellMatrices,
                MatrixName = _context.Matrices.FirstOrDefault(m => m.IdMatrix == id).Name
            });
        }

        private async Task<IActionResult> LoadData(int id)
        {
            Matrix? matrix = _context.Matrices.OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
            if (id == -1 && matrix == null)
                return NotFound();
            if (id != -1 && !_context.Matrices.Any(m => m.IdMatrix == id))
            {
                ViewData["IdUserSegment"] = new SelectList(_context.UserSegments, "IdUserSegment", "Name");
                return View(new CellMatricesViewModel
                {
                    CellMatrices = _cells,
                    IdUserSegment = 0,
                    MatrixName = "Новая матрица"
                });
            }
            else if ((_cells == null || _cells.Count == 0) && id == -1 || (_cells != null && _cells.Count != 0) && _cells.FirstOrDefault().IdMatrix != id)
            {
                var cellMatrices = _context.CellMatrices
                                .Include(c => c.IdCategoryNavigation)
                                .Include(c => c.IdLocationNavigation)
                                .Include(c => c.IdMatrixNavigation)
                                .Where(cm => id > 0 && cm.IdMatrixNavigation.IdMatrix == id
                                            || id == -1 && cm.IdMatrix == matrix.IdMatrix);
                _cells = await cellMatrices.ToListAsync();
                _idMatrix = _cells.FirstOrDefault().IdMatrix;
            }
            var currentMatrix = _context.Matrices.FirstOrDefault(m => m.IdMatrix == _idMatrix);
            return View(new CellMatricesViewModel
            {
                CellMatrices = _cells,
                IdUserSegment = currentMatrix.IdUserSegment,
                MatrixName = currentMatrix.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(CellMatricesViewModel matrixViewModel, string submitButton)
        {
            if (submitButton == "Добавить цену")
            {
                List<CellMatrix> existingCells = new List<CellMatrix>();
                if (matrixViewModel.CellMatrices == null)
                    return RedirectToAction(nameof(Create), new { id = _idMatrix });
                foreach (var item in matrixViewModel.CellMatrices)
                {
                    item.IdLocationNavigation = _context.Locations.FirstOrDefault(l => l.IdLocation == item.IdLocation);
                    item.IdCategoryNavigation = _context.Categories.FirstOrDefault(l => l.IdCategory == item.IdCategory);
                    item.IdMatrixNavigation = _context.Matrices.FirstOrDefault(l => l.IdMatrix == item.IdMatrix);
                    existingCells.Add(item);
                }
                _cells = existingCells;
                return RedirectToAction(nameof(Create), new { id = _idMatrix });
            }
            else
            {
                matrixViewModel.ErrorMessage = string.Empty;
                if (matrixViewModel.CellMatrices == null)
                {
                    matrixViewModel.ErrorMessage = "Матрица пуста";
                    return View(matrixViewModel);
                }
                else if (matrixViewModel.CellMatrices.Any(cm => cm.Price == null))
                {
                    matrixViewModel.ErrorMessage = "Цены указаны неверно";
                    foreach (var item in matrixViewModel.CellMatrices.ToList())
                    {
                        item.IdLocationNavigation = _context.Locations.FirstOrDefault(l => l.IdLocation == item.IdLocation);
                        item.IdCategoryNavigation = _context.Categories.FirstOrDefault(l => l.IdCategory == item.IdCategory);
                        item.IdMatrixNavigation = _context.Matrices.FirstOrDefault(l => l.IdMatrix == item.IdMatrix);
                    }
                    return View(matrixViewModel);
                }
                var matrix = new Matrix
                {
                    Name = (matrixViewModel.IdUserSegment == null ? "baseline" : "discountline") + (_context.Matrices.OrderBy(m => m.IdMatrix).LastOrDefault().IdMatrix + 1),
                    IdUserSegment = matrixViewModel.IdUserSegment,
                };
                _context.Matrices.Add(matrix);
                _context.SaveChanges();
                int id = _context.Matrices.FirstOrDefault(m => matrix.Name == m.Name).IdMatrix;
                foreach (var item in matrixViewModel.CellMatrices)
                {
                    item.IdMatrix = id;
                    item.IdCellMatrix = 0;
                }
                _context.CellMatrices.AddRange(matrixViewModel.CellMatrices);
                _context.SaveChanges();
                _cells = null;
                return await LoadData(matrixViewModel.CellMatrices.FirstOrDefault().IdMatrix);
            }
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
        public IActionResult Create(int id)
        {
            ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "Name");
            ViewData["IdLocation"] = new SelectList(_context.Locations, "IdLocation", "Name");
            ViewData["IdMatrix"] = new SelectList(_context.Matrices, "IdMatrix", "IdMatrix");
            var cellMatrix = new CellMatrix
            {
                IdMatrix = id
            };
            return View(cellMatrix);
        }

        // POST: CellMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCellMatrix,Price,IdLocation,IdCategory,IdMatrix")] CellMatrix cellMatrix)
        {
            if (_cells.Any(cm => cm.IdCategory == cellMatrix.IdCategory && cm.IdLocation == cellMatrix.IdLocation))
            {
                cellMatrix.ErrorMessage = "Такая уже цена существует";
                ViewData["IdCategory"] = new SelectList(_context.Categories, "IdCategory", "Name");
                ViewData["IdLocation"] = new SelectList(_context.Locations, "IdLocation", "Name");
                ViewData["IdMatrix"] = new SelectList(_context.Matrices, "IdMatrix", "IdMatrix");
                return View(cellMatrix);
            }
            cellMatrix.ErrorMessage = "";
            List<CellMatrix> existingCells = _cells ?? new List<CellMatrix>();
            cellMatrix.IdLocationNavigation = _context.Locations.FirstOrDefault(l => l.IdLocation == cellMatrix.IdLocation);
            cellMatrix.IdCategoryNavigation = _context.Categories.FirstOrDefault(l => l.IdCategory == cellMatrix.IdCategory);
            cellMatrix.IdMatrixNavigation = _context.Matrices.FirstOrDefault(l => l.IdMatrix == cellMatrix.IdMatrix);
            existingCells.Add(cellMatrix);
            _cells = existingCells;
            return RedirectToAction(nameof(Index), "CellMatrices", new { id = cellMatrix.IdMatrix });
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

            var cellMatrix = _cells.FirstOrDefault(c => c.IdCellMatrix == id);
            if (cellMatrix == null)
            {
                return NotFound();
            }
            List<CellMatrix> existingCells = _cells ?? new List<CellMatrix>();
            existingCells.Remove(cellMatrix);
            _cells = existingCells;

            return RedirectToAction(nameof(Index), new { id = cellMatrix.IdMatrix });
        }

        // POST: CellMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cellMatrix = _cells.FirstOrDefault(c => c.IdCellMatrix == id);
            if (cellMatrix != null)
            {
                List<CellMatrix> existingCells = _cells ?? new List<CellMatrix>();
                existingCells.Remove(cellMatrix);
                _cells = existingCells;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CellMatrixExists(int id)
        {
            return _context.CellMatrices.Any(e => e.IdCellMatrix == id);
        }
    }
}
