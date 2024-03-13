using AviPriceUI.Data;
using AviPriceUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProtoBuf;
using Serializer = ProtoBuf.Serializer;

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

        public List<string> ServersList
        {
            get => new List<string>
            {
                "https://localhost:7138/",
                "http://94.241.169.171:32777"
            };
        }

        // GET: Matrices/Index
        public async Task<IActionResult> Index(int? id)
        {
            try
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
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        // GET: Matrices/CreateStorage
        public async Task<IActionResult> CreateStorage()
        {
            try
            {
                var matrices = _context.Matrices.Include(m => m.IdUserSegmentNavigation);
                _matrices = await matrices.ToListAsync();
                var matricesViewModel = new MatricesViewModel
                {
                    Matrices = _matrices
                };
                return View(matricesViewModel);
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        // POST: Matrices/CreateStorage
        [HttpPost]
        public async Task<IActionResult> CreateStorage(MatricesViewModel matricesViewModel, string submitButton)
        {
            try
            {
                matricesViewModel.Message = "";
                MatricesUpdate(matricesViewModel.Matrices.ToList());
                if (submitButton == "Искать")
                {
                    matricesViewModel.Matrices = _matrices
                        .Where(m => m.Name.Contains(matricesViewModel.SearchNameText))
                        .Where(m => m.IdUserSegmentNavigation == null || m.IdUserSegmentNavigation.Name.Contains(matricesViewModel.SearchUserSegmentText));
                }
                else if (submitButton == "Отправить сторадж")
                {
                    var selectedMatrices = _matrices.Where(m => m.IsSelected).ToList();
                    if (selectedMatrices.Count == 0)
                        matricesViewModel.Message = "Вы ничего не выбрали";
                    else if (!selectedMatrices.Any(sm => sm.IdUserSegment == null))
                        matricesViewModel.Message = "В сторадже должна быть хотя бы одна матрица";
                    else if (CheckBaselineCount(selectedMatrices))
                        matricesViewModel.Message = "В сторадже может быть только один базлайн";
                    else
                    {
                        var matrixList = new List<Matrix>();
                        foreach (var matrix in selectedMatrices)
                        {
                            matrixList.Add(new Matrix
                            {
                                IdMatrix = matrix.IdMatrix,
                                IdUserSegment = matrix.IdUserSegment,
                                CellMatrices = _context.CellMatrices.Where(cm => cm.IdMatrix == matrix.IdMatrix).ToList(),
                            });
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            foreach (var matrix in matrixList)
                                Serializer.SerializeWithLengthPrefix(memoryStream, matrix, PrefixStyle.Fixed32); // Использован ProtoBuf, потому что быстрее JSON в 2 раза
                            var byteArray = memoryStream.ToArray();
                            HttpClient httpClient;
                            foreach (var url in ServersList)
                            {
                                httpClient = new HttpClient { BaseAddress = new Uri(url) };
                                var request = await httpClient.PostAsJsonAsync("/Storages/Update", byteArray);
                                if (request.StatusCode == System.Net.HttpStatusCode.OK)
                                    matricesViewModel.Message = "Отправлено";
                                else
                                    matricesViewModel.Message = "Ошибка при отправке";
                            }
                        }
                    }
                }
                return View(matricesViewModel);
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        /// <summary>
        /// Обновляет _matrices из списка matrices
        /// </summary>
        /// <param name="matrices"></param>
        private void MatricesUpdate(List<Matrix> matrices)
        {
            foreach (var item in _matrices)
            {
                var matrix = matrices.FirstOrDefault(m => m.Name == item.Name);
                if (matrix != null)
                    item.IsSelected = matrix.IsSelected;
            }
        }

        /// <summary>
        /// Проверяет количество базлайнов
        /// </summary>
        /// <param name="matrices">Список с изменениями</param>
        /// <returns>Возвращает true, если базлайнов больше одного. Возвращает false, если базлайнов меньше одного</returns>
        public bool CheckBaselineCount(List<Matrix> matrices)
        {
            return matrices.Count(m => m.IdUserSegment == null) > 1;
        }

        // GET: Matrices/Create
        public IActionResult Create()
        {
            try
            {
                var idMatrix = _context.Matrices.OrderBy(m => m.IdMatrix).LastOrDefault().IdMatrix + 1;
                return RedirectToAction("Index", "CellMatrices", new { id = idMatrix });
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }

        // POST: Matrices/Index
        [HttpPost]
        public async Task<IActionResult> Index(int? id, MatricesViewModel matricesViewModel)
        {
            try
            {
                var matrices = _context.Matrices
                .Include(m => m.IdUserSegmentNavigation)
                .Where(m => id != 0 || id == 0 && m.IdUserSegment != null)
                .Where(m => m.Name.Contains(matricesViewModel.SearchNameText))
                .Where(m => m.IdUserSegmentNavigation == null
                || m.IdUserSegmentNavigation.Name.Contains(matricesViewModel.SearchUserSegmentText));
                var matriesList = await matrices.ToListAsync();
                matricesViewModel.Matrices = matriesList;
                if (id == 0)
                    matricesViewModel.MatricesType = "Скидочные матрицы";
                else
                    matricesViewModel.MatricesType = "История матриц";
                return View(matricesViewModel);
            }
            catch
            {
                return RedirectToAction("Error", "Main");
            }
        }
    }
}
