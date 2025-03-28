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
        // GET: Admin/SanPhamDichVuKhacs
        public async Task<IActionResult> Index(int? idLoaiDichVuKhac)
        {
            var query = _context.SanPhamDichVuKhacs
                .Include(s => s.IdloaiDichVuKhacNavigation)
                .AsQueryable();

            // Lọc theo loại dịch vụ nếu có chọn
            if (idLoaiDichVuKhac.HasValue)
            {
                query = query.Where(s => s.IdloaiDichVuKhac == idLoaiDichVuKhac);
            }

            // Load danh sách loại dịch vụ khác để hiển thị bộ lọc
            ViewBag.LoaiDichVuKhac = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", idLoaiDichVuKhac);

            return View(await query.ToListAsync());
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
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu");
            return View();
        }

        //upload file
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\dichvu\\dichvukhac\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }

        // POST: Admin/SanPhamDichVuKhacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile HinhAnh, [Bind("IdsanPham,TenSanPham,HinhAnh,MoTa,ThongTinChiTiet,IdloaiDichVuKhac")] SanPhamDichVuKhac sanPhamDichVuKhac)
        {
            if (ModelState.IsValid)
            {
                // ✨ Xử lý giữ nguyên xuống dòng và thụt đầu dòng
                sanPhamDichVuKhac.MoTa = sanPhamDichVuKhac.MoTa?.Replace("\n", "<br>");
                sanPhamDichVuKhac.ThongTinChiTiet = sanPhamDichVuKhac.ThongTinChiTiet?.Replace("\n", "<br>");

                //upload ảnh vào
                sanPhamDichVuKhac.HinhAnh = Upload(HinhAnh);
                _context.Add(sanPhamDichVuKhac);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", sanPhamDichVuKhac.IdloaiDichVuKhac);
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
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", sanPhamDichVuKhac.IdloaiDichVuKhac);
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
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", sanPhamDichVuKhac.IdloaiDichVuKhac);
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
