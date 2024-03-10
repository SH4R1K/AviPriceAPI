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
    public class HistoryController : Controller
    {
        private readonly AviContext _context;

        public HistoryController(AviContext context)
        {
            _context = context;
        }

        // GET: History
        public async Task<IActionResult> Index()
        {
            var aviContext = _context.Matrices.Include(m => m.IdUserSegmentNavigation);
            return View(await aviContext.ToListAsync());
        }

        // GET: History/Details/5
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

        private bool MatrixExists(int id)
        {
            return _context.Matrices.Any(e => e.IdMatrix == id);
        }
    }
}
