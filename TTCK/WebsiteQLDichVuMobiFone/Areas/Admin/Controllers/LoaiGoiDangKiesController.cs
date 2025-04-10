﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class LoaiGoiDangKiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiGoiDangKiesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            GetData(); // Gọi hàm lấy dữ liệu
            base.OnActionExecuting(context);
        }
        public void GetData()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa bằng cách kiểm tra session "nguoidung"
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");

            if (!string.IsNullOrEmpty(tenDangNhap))
            {
                // Tìm người dùng từ cơ sở dữ liệu bằng tên đăng nhập đã lưu trong session
                ViewBag.khachHang = _context.NguoiDungs.FirstOrDefault(k => k.TenDangNhap == tenDangNhap);
            }
            // Truyền thông tin vào ViewData hoặc ViewBag
            ViewBag.UserName = tenDangNhap;
            ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
        }
        // GET: Admin/LoaiGoiDangKies
        // GET: Admin/LoaiGoiDangKies
        public async Task<IActionResult> Index(int? idLoaiDichVu, int? idLoaiGoi)
        {
            var loaiGoiDangKies = _context.LoaiGoiDangKies
                .Include(l => l.IdloaiDichVuNavigation)
                .AsQueryable();

            if (idLoaiDichVu.HasValue)
            {
                loaiGoiDangKies = loaiGoiDangKies.Where(lg => lg.IdloaiDichVu == idLoaiDichVu);
            }

            if (idLoaiGoi.HasValue)
            {
                loaiGoiDangKies = loaiGoiDangKies.Where(lg => lg.IdloaiGoi == idLoaiGoi);
            }

            ViewData["LoaiDichVus"] = new SelectList(_context.LoaiDichVuDiDongs, "IdloaiDichVu", "TenLoaiDichVu");
            ViewData["LoaiGoiDangKies"] = new SelectList(_context.LoaiGoiDangKies, "IdloaiGoi", "TenLoaiGoi");

            return View(await loaiGoiDangKies.ToListAsync());
        }
        public async Task<JsonResult> GetLoaiGoiByDichVu(int idLoaiDichVu)
        {
            var loaiGoiList = await _context.LoaiGoiDangKies
                .Where(lg => lg.IdloaiDichVu == idLoaiDichVu)
                .Select(lg => new { lg.IdloaiGoi, lg.TenLoaiGoi })
                .ToListAsync();

            return Json(loaiGoiList);
        }


        // GET: Admin/LoaiGoiDangKies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiGoiDangKy = await _context.LoaiGoiDangKies
                .Include(lg => lg.IdloaiDichVuNavigation)
                .Include(lg => lg.GoiDangKies) // Lấy thêm danh sách các gói đăng ký liên quan
                .FirstOrDefaultAsync(m => m.IdloaiGoi == id);

            if (loaiGoiDangKy == null)
            {
                return NotFound();
            }

            return View(loaiGoiDangKy);
        }


        // GET: Admin/LoaiGoiDangKies/Create
        public IActionResult Create()
        {
            ViewData["IdloaiDichVu"] = new SelectList(_context.LoaiDichVuDiDongs, "IdloaiDichVu", "TenLoaiDichVu");
            return View();
        }

        // POST: Admin/LoaiGoiDangKies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdloaiGoi,TenLoaiGoi,IdloaiDichVu")] LoaiGoiDangKy loaiGoiDangKy)
        {
            if (await _context.LoaiGoiDangKies.AnyAsync(g => g.TenLoaiGoi == loaiGoiDangKy.TenLoaiGoi))
            {
                ModelState.AddModelError("TenLoaiGoi", "Tên loại gói đăng ký này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(loaiGoiDangKy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdloaiDichVu"] = new SelectList(_context.LoaiDichVuDiDongs, "IdloaiDichVu", "TenLoaiDichVu", loaiGoiDangKy.IdloaiDichVu);
            return View(loaiGoiDangKy);
        }

        // GET: Admin/LoaiGoiDangKies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiGoiDangKy = await _context.LoaiGoiDangKies.FindAsync(id);
            if (loaiGoiDangKy == null)
            {
                return NotFound();
            }
            ViewData["IdloaiDichVu"] = new SelectList(_context.LoaiDichVuDiDongs, "IdloaiDichVu", "TenLoaiDichVu", loaiGoiDangKy.IdloaiDichVu);
            return View(loaiGoiDangKy);
        }

        // POST: Admin/LoaiGoiDangKies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdloaiGoi,TenLoaiGoi,IdloaiDichVu")] LoaiGoiDangKy loaiGoiDangKy)
        {
            if (id != loaiGoiDangKy.IdloaiGoi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiGoiDangKy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiGoiDangKyExists(loaiGoiDangKy.IdloaiGoi))
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
            ViewData["IdloaiDichVu"] = new SelectList(_context.LoaiDichVuDiDongs, "IdloaiDichVu", "IdloaTenLoaiDichVuiDichVu", loaiGoiDangKy.IdloaiDichVu);
            return View(loaiGoiDangKy);
        }

        // GET: Admin/LoaiGoiDangKies/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaigoi = await _context.LoaiGoiDangKies.FindAsync(id);
            if (loaigoi != null)
            {
                _context.LoaiGoiDangKies.Remove(loaigoi);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool LoaiGoiDangKyExists(int id)
        {
            return _context.LoaiGoiDangKies.Any(e => e.IdloaiGoi == id);
        }
    }
}
