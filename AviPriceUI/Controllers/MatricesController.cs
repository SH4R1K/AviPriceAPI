﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AviPriceUI.Data;
using AviPriceUI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AviPriceUI.Controllers
{
    public class MatricesController : Controller
    {
        private readonly AviContext _context;

        public MatricesController(AviContext context)
        {
            _context = context;
        }

        // GET: Matrices
        public async Task<IActionResult> Index(int? id)
        {
            var aviApiContext = _context.Matrices.Include(m => m.IdUserSegmentNavigation).Where(m => id != 0 || id == 0 && m.IdUserSegment != null);
            var matriesList = await aviApiContext.ToListAsync();
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
            if (matricesViewModel.SearchNameText == null)
                matricesViewModel.SearchNameText = "";
            if (matricesViewModel.SearchUserSegmentText == null)
                matricesViewModel.SearchUserSegmentText = "";
            var aviApiContext = _context.Matrices
                .Include(m => m.IdUserSegmentNavigation)
                .Where(m => id != 0 || id == 0 && m.IdUserSegment != null)
                .Where(m => m.Name.Contains(matricesViewModel.SearchNameText))
                .Where(m => m.IdUserSegmentNavigation == null && matricesViewModel.SearchUserSegmentText.IsNullOrEmpty() 
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
