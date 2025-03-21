﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaiDichVuKhacsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiDichVuKhacsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoaiDichVuKhacs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LoaiDichVuKhacs.Include(l => l.IddichVuNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/LoaiDichVuKhacs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiDichVuKhac = await _context.LoaiDichVuKhacs
                .Include(l => l.IddichVuNavigation)
                .FirstOrDefaultAsync(m => m.IdloaiDichVuKhac == id);
            if (loaiDichVuKhac == null)
            {
                return NotFound();
            }

            return View(loaiDichVuKhac);
        }

        // GET: Admin/LoaiDichVuKhacs/Create
        public IActionResult Create()
        {
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu");
            return View();
        }

        // POST: Admin/LoaiDichVuKhacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdloaiDichVuKhac,IddichVu,TenLoaiDichVu")] LoaiDichVuKhac loaiDichVuKhac)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiDichVuKhac);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu", loaiDichVuKhac.IddichVu);
            return View(loaiDichVuKhac);
        }

        // GET: Admin/LoaiDichVuKhacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiDichVuKhac = await _context.LoaiDichVuKhacs.FindAsync(id);
            if (loaiDichVuKhac == null)
            {
                return NotFound();
            }
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu", loaiDichVuKhac.IddichVu);
            return View(loaiDichVuKhac);
        }

        // POST: Admin/LoaiDichVuKhacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdloaiDichVuKhac,IddichVu,TenLoaiDichVu")] LoaiDichVuKhac loaiDichVuKhac)
        {
            if (id != loaiDichVuKhac.IdloaiDichVuKhac)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiDichVuKhac);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiDichVuKhacExists(loaiDichVuKhac.IdloaiDichVuKhac))
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
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu", loaiDichVuKhac.IddichVu);
            return View(loaiDichVuKhac);
        }

        // GET: Admin/LoaiDichVuKhacs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiDichVuKhac = await _context.LoaiDichVuKhacs
                .Include(l => l.IddichVuNavigation)
                .FirstOrDefaultAsync(m => m.IdloaiDichVuKhac == id);
            if (loaiDichVuKhac == null)
            {
                return NotFound();
            }

            return View(loaiDichVuKhac);
        }

        // POST: Admin/LoaiDichVuKhacs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiDichVuKhac = await _context.LoaiDichVuKhacs.FindAsync(id);
            if (loaiDichVuKhac != null)
            {
                _context.LoaiDichVuKhacs.Remove(loaiDichVuKhac);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiDichVuKhacExists(int id)
        {
            return _context.LoaiDichVuKhacs.Any(e => e.IdloaiDichVuKhac == id);
        }
    }
}
