using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class LoaiSoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiSoesController(ApplicationDbContext context)
        {
            _context = context;
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
        // GET: Admin/LoaiSoes
        public async Task<IActionResult> Index()
        {
            GetData();
            return View(await _context.LoaiSos.ToListAsync());
        }

        // GET: Admin/LoaiSoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            GetData();
            if (id == null)
            {
                return NotFound();
            }

            var loaiSo = await _context.LoaiSos
                .FirstOrDefaultAsync(m => m.IdloaiSo == id);
            if (loaiSo == null)
            {
                return NotFound();
            }

            return View(loaiSo);
        }

        // GET: Admin/LoaiSoes/Create
        public IActionResult Create()
        {
            GetData();
            return View();
        }

        // POST: Admin/LoaiSoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdloaiSo,TenLoaiSo")] LoaiSo loaiSo)
        {
            GetData();
            if (await _context.LoaiSos.AnyAsync(g => g.TenLoaiSo == loaiSo.TenLoaiSo))
            {
                ModelState.AddModelError("TenLoaiSo", "Thể loại sim này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(loaiSo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSo);
        }

        // GET: Admin/LoaiSoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            GetData();
            if (id == null)
            {
                return NotFound();
            }

            var loaiSo = await _context.LoaiSos.FindAsync(id);
            if (loaiSo == null)
            {
                return NotFound();
            }
            return View(loaiSo);
        }

        // POST: Admin/LoaiSoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdloaiSo,TenLoaiSo")] LoaiSo loaiSo)
        {
            GetData();
            if (id != loaiSo.IdloaiSo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSoExists(loaiSo.IdloaiSo))
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
            return View(loaiSo);
        }

        // GET: Admin/LoaiSoes/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiso = await _context.LoaiSos.FindAsync(id);
            if (loaiso != null)
            {
                _context.LoaiSos.Remove(loaiso);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool LoaiSoExists(int id)
        {
            return _context.LoaiSos.Any(e => e.IdloaiSo == id);
        }
    }
}
