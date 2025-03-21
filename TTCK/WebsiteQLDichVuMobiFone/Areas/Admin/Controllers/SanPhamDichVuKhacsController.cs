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
    public class SanPhamDichVuKhacsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamDichVuKhacsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SanPhamDichVuKhacs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SanPhamDichVuKhacs.Include(s => s.IdloaiDichVuKhacNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/SanPhamDichVuKhacs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPhamDichVuKhac = await _context.SanPhamDichVuKhacs
                .Include(s => s.IdloaiDichVuKhacNavigation)
                .FirstOrDefaultAsync(m => m.IdsanPham == id);
            if (sanPhamDichVuKhac == null)
            {
                return NotFound();
            }

            return View(sanPhamDichVuKhac);
        }

        // GET: Admin/SanPhamDichVuKhacs/Create
        public IActionResult Create()
        {
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "IdloaiDichVuKhac");
            return View();
        }

        // POST: Admin/SanPhamDichVuKhacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdsanPham,TenSanPham,HinhAnh,MoTa,ThongTinChiTiet,IdloaiDichVuKhac")] SanPhamDichVuKhac sanPhamDichVuKhac)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPhamDichVuKhac);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "IdloaiDichVuKhac", sanPhamDichVuKhac.IdloaiDichVuKhac);
            return View(sanPhamDichVuKhac);
        }

        // GET: Admin/SanPhamDichVuKhacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPhamDichVuKhac = await _context.SanPhamDichVuKhacs.FindAsync(id);
            if (sanPhamDichVuKhac == null)
            {
                return NotFound();
            }
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "IdloaiDichVuKhac", sanPhamDichVuKhac.IdloaiDichVuKhac);
            return View(sanPhamDichVuKhac);
        }

        // POST: Admin/SanPhamDichVuKhacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdsanPham,TenSanPham,HinhAnh,MoTa,ThongTinChiTiet,IdloaiDichVuKhac")] SanPhamDichVuKhac sanPhamDichVuKhac)
        {
            if (id != sanPhamDichVuKhac.IdsanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPhamDichVuKhac);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamDichVuKhacExists(sanPhamDichVuKhac.IdsanPham))
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
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "IdloaiDichVuKhac", sanPhamDichVuKhac.IdloaiDichVuKhac);
            return View(sanPhamDichVuKhac);
        }

        // GET: Admin/SanPhamDichVuKhacs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPhamDichVuKhac = await _context.SanPhamDichVuKhacs
                .Include(s => s.IdloaiDichVuKhacNavigation)
                .FirstOrDefaultAsync(m => m.IdsanPham == id);
            if (sanPhamDichVuKhac == null)
            {
                return NotFound();
            }

            return View(sanPhamDichVuKhac);
        }

        // POST: Admin/SanPhamDichVuKhacs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPhamDichVuKhac = await _context.SanPhamDichVuKhacs.FindAsync(id);
            if (sanPhamDichVuKhac != null)
            {
                _context.SanPhamDichVuKhacs.Remove(sanPhamDichVuKhac);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamDichVuKhacExists(int id)
        {
            return _context.SanPhamDichVuKhacs.Any(e => e.IdsanPham == id);
        }
    }
}
