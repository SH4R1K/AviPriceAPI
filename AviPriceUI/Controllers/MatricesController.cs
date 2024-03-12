using AviPriceUI.Data;
using AviPriceUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AviPriceUI.Controllers
{
    public class MatricesController : Controller
    {
        private readonly AviContext _context;

        IMemoryCache cache;

        public MatricesController(AviContext context, IMemoryCache memoryCache)
        {
            _context = context;
            cache = memoryCache;
        }

        private List<Matrix>? _matrices
        {
            get
            {
                return cache.Get("CellMatrixList") as List<Matrix>;
            }
            set
            {
                cache.Set("CellMatrixList", value, DateTimeOffset.Now.AddDays(7));
            }
        }

        // GET: Matrices
        public async Task<IActionResult> Index(int? id)
        {
            var matrices = _context.Matrices.Include(m => m.IdUserSegmentNavigation).Where(m => id != 0 || id == 0 && m.IdUserSegment != null);
            var matriesList = await matrices.ToListAsync();
            var matricesViewModel = new MatricesViewModel
            {
                Matrices = matriesList
            };
            if (id == 0)
                matricesViewModel.MatricesType = "Скидочные матрицы";
            else
                matricesViewModel.MatricesType = "История матриц";
            return View(matricesViewModel);
        }

        public async Task<IActionResult> CreateStorage()
        {
            var matrices = _context.Matrices.Include(m => m.IdUserSegmentNavigation);
            _matrices = await matrices.ToListAsync();
            var matricesViewModel = new MatricesViewModel
            {
                Matrices = _matrices
            };
            return View(matricesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStorage(MatricesViewModel matricesViewModel, string submitButton)
        {
            matricesViewModel.ErrorMessage = "";
            MatricesUpdate(matricesViewModel.Matrices.ToList());
            if (submitButton == "Искать")
            {
                matricesViewModel.Matrices = _matrices.Where(m => m.Name.Contains(matricesViewModel.SearchNameText))
                    .Where(m => m.IdUserSegmentNavigation == null || m.IdUserSegmentNavigation.Name.Contains(matricesViewModel.SearchUserSegmentText));
            }
            else if (submitButton == "Отправить сторадж")
            {
                var selectedMatrices = _matrices.Where(m => m.IsSelected).ToList();
                if (selectedMatrices.Count == 0)
                    matricesViewModel.ErrorMessage = "Вы ничего не выбрали";
                else if (CheckBaselineCount(selectedMatrices))
                    matricesViewModel.ErrorMessage = "В сторадже может быть только один базлайн";
                else
                    matricesViewModel.ErrorMessage = "Отправлено";
            }
            return View(matricesViewModel);
        }

        private void MatricesUpdate(List<Matrix> matrices)
        {
            foreach (var item in _matrices)
            {
                var matrix = matrices.FirstOrDefault(m => m.Name == item.Name);
                if(matrix != null)
                    item.IsSelected = matrix.IsSelected;
            }
        }

        public bool CheckBaselineCount(List<Matrix> matrices)
        {
            int counter = 0;
            foreach (var item in matrices)
            {
                if (item.IdUserSegment == null)
                    counter++;
                if (counter > 1)
                    return true;
            }
            return false;
        }

        // GET: Matrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matrix = await _context.Matrices
                .Include(m => m.IdUserSegmentNavigation)
                .FirstOrDefaultAsync(m => m.IdMatrix == id);
            if (matrix == null)
            {
                return NotFound();
            }

            return View(matrix);
        }

        // GET: Matrices/Create
        public IActionResult Create()
        {
            var idMatrix = _context.Matrices.OrderBy(m => m.IdMatrix).LastOrDefault().IdMatrix + 1;
            return RedirectToAction("Index", "CellMatrices", new { id = idMatrix });
        }

        // POST: Matrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMatrix,Name,IdUserSegment")] Matrix matrix)
        {
            if (ModelState.IsValid)
            {
                _context.Add(matrix);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUserSegment"] = new SelectList(_context.UserSegments, "IdUserSegment", "IdUserSegment", matrix.IdUserSegment);
            return View(matrix);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? id, MatricesViewModel matricesViewModel)
        {
            var aviApiContext = _context.Matrices
                .Include(m => m.IdUserSegmentNavigation)
                .Where(m => id != 0 || id == 0 && m.IdUserSegment != null)
                .Where(m => m.Name.Contains(matricesViewModel.SearchNameText))
                .Where(m => m.IdUserSegmentNavigation == null
                || m.IdUserSegmentNavigation.Name.Contains(matricesViewModel.SearchUserSegmentText));
            var matriesList = await aviApiContext.ToListAsync();
            matricesViewModel.Matrices = matriesList;
            if (id == 0)
                matricesViewModel.MatricesType = "Скидочные матрицы";
            else
                matricesViewModel.MatricesType = "История матриц";
            return View(matricesViewModel);
        }

        // GET: Matrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matrix = await _context.Matrices.FindAsync(id);
            if (matrix == null)
            {
                return NotFound();
            }
            ViewData["IdUserSegment"] = new SelectList(_context.UserSegments, "IdUserSegment", "IdUserSegment", matrix.IdUserSegment);
            return View(matrix);
        }

        // POST: Matrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMatrix,Name,IdUserSegment")] Matrix matrix)
        {
            if (id != matrix.IdMatrix)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matrix);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatrixExists(matrix.IdMatrix))
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
            ViewData["IdUserSegment"] = new SelectList(_context.UserSegments, "IdUserSegment", "IdUserSegment", matrix.IdUserSegment);
            return View(matrix);
        }

        // GET: Matrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matrix = await _context.Matrices
                .Include(m => m.IdUserSegmentNavigation)
                .FirstOrDefaultAsync(m => m.IdMatrix == id);
            if (matrix == null)
            {
                return NotFound();
            }

            return View(matrix);
        }

        // POST: Matrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var matrix = await _context.Matrices.FindAsync(id);
            if (matrix != null)
            {
                _context.Matrices.Remove(matrix);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatrixExists(int id)
        {
            return _context.Matrices.Any(e => e.IdMatrix == id);
        }
    }
}
