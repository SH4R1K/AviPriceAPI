using AviPriceUI.Data;
using AviPriceUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;

namespace AviPriceUI.Controllers
{
    public class CellMatricesController : Controller
    {
        private readonly AviContext _context;

        IMemoryCache cache;

        private readonly int pageSize = 25; 

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

        private int _pageIndex
        {
            get
            {
                return cache.Get("PageIndex") as int? ?? 1;
            }
            set
            {
                cache.Set("PageIndex", value, DateTimeOffset.Now.AddDays(7));
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

        // GET: CellMatrices/Index/1
        public async Task<IActionResult> Index(int id)
        {
            return await LoadDataAsync(id);
        }

        // GET: CellMatrices/IndexNotEdit/1
        public async Task<IActionResult> IndexNotEdit(int id)
        {
            try
            {
                var cellMatrices = await LoadCellMatricesAsync(_idMatrix ?? 1);
                _idMatrix = id;
                _pageIndex = 1;
                return View(new CellMatricesViewModel
                {
                    CellMatrices = cellMatrices.Take(pageSize),
                    MatrixName = (await _context.Matrices.FirstOrDefaultAsync(m => m.IdMatrix == id)).Name,
                    PageCount = (int)Math.Ceiling((double)cellMatrices.Count / pageSize),
                    PageIndex = _pageIndex
                });
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        //POST: CellMatrices/IndexNotEdit
        [HttpPost]
        public async Task<IActionResult> IndexNotEdit(CellMatricesViewModel cellMatricesViewModel, string submitButton)
        {
            try
            {
                List<CellMatrix> cellMatrices = await LoadCellMatricesAsync(_idMatrix ?? 1, cellMatricesViewModel.SearchCategoryText, cellMatricesViewModel.SearchLocationText);
                int pageCount = (int)Math.Ceiling((double)cellMatrices.Count / pageSize);
                GoToPage(submitButton, pageCount);

                return View(new CellMatricesViewModel
                {
                    CellMatrices = cellMatrices.Skip((_pageIndex - 1) * pageSize).Take(pageSize),
                    MatrixName = (await _context.Matrices.FirstOrDefaultAsync(m => m.IdMatrix == _idMatrix)).Name,
                    PageCount = pageCount,
                    PageIndex = _pageIndex
                });
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        private async Task<List<CellMatrix>> LoadCellMatricesAsync(int id, string searchCategoryText = "", string searchLocationText = "")
        {
            return await _context.CellMatrices
                .Include(cm => cm.IdCategoryNavigation)
                .Include(cm => cm.IdLocationNavigation)
                .Where(cm => cm.IdMatrix == _idMatrix)
                .Where(cm => cm.IdCategoryNavigation.Name.Contains(searchCategoryText))
                .Where(cm => cm.IdLocationNavigation.Name.Contains(searchLocationText))
                .ToListAsync();
        }

        /// <summary>
        /// Убавляет или прибавляет _pageIndex на 1 взависимости от нажатой кнопки
        /// </summary>
        /// <param name="submitButton">Кнопка перехода</param>
        /// <param name="pageCount">Количество страниц</param>
        private void GoToPage(string submitButton, int pageCount)
        {
            if (submitButton == "Назад" && _pageIndex > 1)
                _pageIndex--;
            if (submitButton == "Вперед" && _pageIndex < pageCount)
                _pageIndex++;
        }

        /// <summary>
        /// Загружает данные для страницы Index
        /// </summary>
        /// <param name="id">Индекс матрицы. Если -1, то последнюю, если не существующий индекс - новая матрица</param>
        /// <returns>Страница Index</returns>
        private async Task<IActionResult> LoadDataAsync(int id)
        {
            try
            {
                if (id == 0)
                    id = _idMatrix ?? 0;
                CellMatricesViewModel cellMatricesViewModel;
                Matrix? matrix = _context.Matrices.OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);

                if (id == -1 && matrix == null)
                    return NotFound();

                if (id != -1 && !_context.Matrices.Any(m => m.IdMatrix == id))
                {
                    ViewData["IdUserSegment"] = new SelectList(_context.UserSegments, "IdUserSegment", "Name");
                    _idMatrix = id;
                    if (_cells != null && !_cells.All(m => m.IdMatrix == id))
                        _cells = null;
                    cellMatricesViewModel = new CellMatricesViewModel
                    {
                        CellMatrices = _cells,
                        IdUserSegment = 0,
                        MatrixName = "Новая матрица"
                    };
                    return View(cellMatricesViewModel);
                }
                else if ((_cells == null || _cells.Count == 0) || _cells.FirstOrDefault().IdMatrix != id)
                {
                    var cellMatrices = _context.CellMatrices
                                    .Include(c => c.IdCategoryNavigation)
                                    .Include(c => c.IdLocationNavigation)
                                    .Include(c => c.IdMatrixNavigation)
                                    .Where(cm => id > 0 && cm.IdMatrixNavigation.IdMatrix == id
                                                || id == -1 && cm.IdMatrix == matrix.IdMatrix);
                    _cells = await cellMatrices.ToListAsync();
                    _idMatrix = _cells.FirstOrDefault().IdMatrix;
                    _pageIndex = 1;
                }

                var currentMatrix = _context.Matrices.FirstOrDefault(m => m.IdMatrix == _idMatrix);
                cellMatricesViewModel = new CellMatricesViewModel
                {
                    CellMatrices = _cells.Skip((_pageIndex - 1) * pageSize)
                                        .Take(pageSize),
                    IdUserSegment = currentMatrix.IdUserSegment,
                    MatrixName = currentMatrix.Name,
                    PageCount = (int)Math.Ceiling((double)_cells.Count / pageSize),
                    PageIndex = _pageIndex
                };

                return View(cellMatricesViewModel);
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        //POST: CellMatrices/Index
        [HttpPost]
        public async Task<IActionResult> Index(CellMatricesViewModel matrixViewModel, string submitButton)
        {
            try
            {
                if (submitButton == "Добавить цену")
                {
                    List<CellMatrix> existingCells = new List<CellMatrix>();
                    if (matrixViewModel.CellMatrices == null)
                        return RedirectToAction(nameof(Create), new { id = _idMatrix });

                    LoadCellMatricesViewData(matrixViewModel);
                    existingCells.AddRange(matrixViewModel.CellMatrices);
                    _cells = existingCells;

                    return RedirectToAction(nameof(Create), new { id = _idMatrix });
                }
                else if (submitButton == "Искать")
                {
                    Matrix? matrix = _context.Matrices.OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
                    if (!_context.Matrices.Any(m => m.IdMatrix == _idMatrix))
                    {
                        ViewData["IdUserSegment"] = new SelectList(_context.UserSegments, "IdUserSegment", "Name");
                        if (!_cells.All(m => m.IdMatrix == _idMatrix))
                            _cells = null;
                        var cellMatricesViewModel = new CellMatricesViewModel
                        {
                            CellMatrices = _cells,
                            IdUserSegment = 0,
                            MatrixName = "Новая матрица"
                        };
                        return View(cellMatricesViewModel);
                    }
                    else if ((_cells == null || _cells.Count == 0) || _cells.FirstOrDefault().IdMatrix != _idMatrix)
                    {
                        var cellMatrices = _context.CellMatrices
                                        .Include(c => c.IdCategoryNavigation)
                                        .Include(c => c.IdLocationNavigation)
                                        .Include(c => c.IdMatrixNavigation)
                                        .Where(cm => cm.IdMatrixNavigation.IdMatrix == _idMatrix);
                        _cells = await cellMatrices.ToListAsync();
                        _pageIndex = 1;
                    }
                    var currentMatrix = _context.Matrices.FirstOrDefault(m => m.IdMatrix == _idMatrix);
                    return View(new CellMatricesViewModel
                    {
                        CellMatrices = _cells.Where(cm => cm.IdCategoryNavigation.Name.Contains(matrixViewModel.SearchCategoryText))
                                        .Where(cm => cm.IdLocationNavigation.Name.Contains(matrixViewModel.SearchLocationText))
                                        .Skip((_pageIndex - 1) * pageSize)
                                        .Take(pageSize),
                        IdUserSegment = currentMatrix.IdUserSegment,
                        MatrixName = currentMatrix.Name,
                        PageCount = (int)Math.Ceiling((double)_cells.Where(cm => cm.IdCategoryNavigation.Name.Contains(matrixViewModel.SearchCategoryText))
                                        .Where(cm => cm.IdLocationNavigation.Name.Contains(matrixViewModel.SearchLocationText)).Count() / pageSize),
                        PageIndex = 1
                    });
                }
                else if (submitButton == "Сохранить")
                {
                    matrixViewModel.Message = "Сохранено";
                    if (matrixViewModel.CellMatrices == null)
                    {
                        matrixViewModel.Message = "Матрица пуста";
                        return View(matrixViewModel);
                    }
                    else if (matrixViewModel.CellMatrices.Any(cm => cm.Price == null))
                    {
                        matrixViewModel.Message = "Цены указаны неверно";
                        LoadCellMatricesViewData(matrixViewModel);
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
                    CellsUpdate(matrixViewModel, id);
                    foreach (var item in _cells)
                    {
                        item.IdCategoryNavigation = null;
                        item.IdLocationNavigation = null;
                        item.IdMatrixNavigation = null;
                    }
                    _context.CellMatrices.AddRange(_cells);
                    _context.SaveChanges();
                    _cells = null;
                    return await LoadDataAsync(_context.Matrices.FirstOrDefault(m => m.Name == matrix.Name).IdMatrix);
                }
                else
                {
                    CellsUpdate(matrixViewModel, matrixViewModel.CellMatrices.FirstOrDefault().IdMatrix);
                    LoadCellMatricesViewData(matrixViewModel);
                    int pageCount = (int)Math.Ceiling((double)_cells.Where(cm => cm.IdCategoryNavigation.Name.Contains(matrixViewModel.SearchCategoryText))
                                        .Where(cm => cm.IdLocationNavigation.Name.Contains(matrixViewModel.SearchLocationText)).Count() / pageSize);
                    GoToPage(submitButton, pageCount);
                    var currentMatrix = _context.Matrices.FirstOrDefault(m => m.IdMatrix == _idMatrix);
                    return View(new CellMatricesViewModel
                    {
                        CellMatrices = _cells.Where(cm => cm.IdCategoryNavigation.Name.Contains(matrixViewModel.SearchCategoryText))
                                        .Where(cm => cm.IdLocationNavigation.Name.Contains(matrixViewModel.SearchLocationText))
                                        .Skip((_pageIndex - 1) * pageSize)
                                        .Take(pageSize),
                        IdUserSegment = currentMatrix.IdUserSegment,
                        MatrixName = currentMatrix.Name,
                        PageCount = pageCount,
                        PageIndex = _pageIndex
                    });
                }
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        /// <summary>
        /// Догружает данные о категории, локации, матрице
        /// </summary>
        /// <param name="matrixViewModel">ViewModel матрицы пришедший со страницы</param>
        private void LoadCellMatricesViewData(CellMatricesViewModel matrixViewModel)
        {
            foreach (var item in matrixViewModel.CellMatrices)
            {
                item.IdLocationNavigation = _context.Locations.FirstOrDefault(l => l.IdLocation == item.IdLocation);
                item.IdCategoryNavigation = _context.Categories.FirstOrDefault(l => l.IdCategory == item.IdCategory);
                item.IdMatrixNavigation = _context.Matrices.FirstOrDefault(l => l.IdMatrix == item.IdMatrix);
            }
        }

        /// <summary>
        /// Обновляет _cells с учетом изменении из matrixViewModel
        /// </summary>
        /// <param name="matrixViewModel">ViewModel матрицы пришедший со страницы</param>
        /// <param name="id">Индекс матрицы</param>
        private void CellsUpdate(CellMatricesViewModel matrixViewModel, int id)
        {
            foreach (var item in _cells)
            {
                item.IdMatrix = id;
                item.IdCellMatrix = 0;
                var cellMatrix = matrixViewModel.CellMatrices.FirstOrDefault(c => c.IdCategory == item.IdCategory && c.IdLocation == item.IdLocation);
                if (cellMatrix != null)
                    item.Price = cellMatrix.Price;
            }
        }

        // GET: CellMatrices/Create
        public IActionResult Create(int id)
        {
            try
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
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        // POST: CellMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCellMatrix,Price,IdLocation,IdCategory,IdMatrix")] CellMatrix cellMatrix)
        {
            try
            {
                if (_cells != null && _cells.Any(cm => cm.IdCategory == cellMatrix.IdCategory && cm.IdLocation == cellMatrix.IdLocation))
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
                cellMatrix.IdMatrix = _idMatrix ?? cellMatrix.IdMatrix;
                existingCells.Add(cellMatrix);
                _cells = existingCells;
                return RedirectToAction(nameof(Index), "CellMatrices", new { id = _idMatrix });
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        // GET: CellMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
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
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }
    }
}
