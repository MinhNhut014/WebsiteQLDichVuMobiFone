using System;
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
    public class GoiDangKyDichVuKhacsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoiDangKyDichVuKhacsController(ApplicationDbContext context)
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
        // GET: Admin/GoiDangKyDichVuKhacs
        public async Task<IActionResult> Index(int? loiGoi)
        {
            var applicationDbContext = _context.GoiDangKyDichVuKhacs
                .Include(g => g.IdsanPhamNavigation)
            .OrderByDescending(s => s.IdgoiDangKy); // Sắp xếp mới nhất
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/GoiDangKyDichVuKhacs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDangKyDichVuKhac = await _context.GoiDangKyDichVuKhacs
                .Include(g => g.IdsanPhamNavigation)
                .FirstOrDefaultAsync(m => m.IdgoiDangKy == id);
            if (goiDangKyDichVuKhac == null)
            {
                return NotFound();
            }

            return View(goiDangKyDichVuKhac);
        }

        // GET: Admin/GoiDangKyDichVuKhacs/Create
        public IActionResult Create()
        {
            ViewData["IdsanPham"] = new SelectList(_context.SanPhamDichVuKhacs, "IdsanPham", "TenSanPham");
            return View();
        }

        // POST: Admin/GoiDangKyDichVuKhacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdgoiDangKy,TenGoi,GiaGoi,ThoiHan,IdsanPham")] GoiDangKyDichVuKhac goiDangKyDichVuKhac)
        {
            if (await _context.GoiDangKyDichVuKhacs.AnyAsync(g => g.TenGoi == goiDangKyDichVuKhac.TenGoi))
            {
                ModelState.AddModelError("TenGoi", "Tên gói đăng ký này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(goiDangKyDichVuKhac);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdsanPham"] = new SelectList(_context.SanPhamDichVuKhacs, "IdsanPham", "TenSanPham", goiDangKyDichVuKhac.IdsanPham);
            return View(goiDangKyDichVuKhac);
        }

        // GET: Admin/GoiDangKyDichVuKhacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDangKyDichVuKhac = await _context.GoiDangKyDichVuKhacs.FindAsync(id);
            if (goiDangKyDichVuKhac == null)
            {
                return NotFound();
            }
            ViewData["IdsanPham"] = new SelectList(_context.SanPhamDichVuKhacs, "IdsanPham", "TenSanPham", goiDangKyDichVuKhac.IdsanPham);
            return View(goiDangKyDichVuKhac);
        }

        // POST: Admin/GoiDangKyDichVuKhacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdgoiDangKy,TenGoi,GiaGoi,ThoiHan,IdsanPham")] GoiDangKyDichVuKhac goiDangKyDichVuKhac)
        {
            if (id != goiDangKyDichVuKhac.IdgoiDangKy)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goiDangKyDichVuKhac);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoiDangKyDichVuKhacExists(goiDangKyDichVuKhac.IdgoiDangKy))
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
            ViewData["IdsanPham"] = new SelectList(_context.SanPhamDichVuKhacs, "IdsanPham", "TenSanPham", goiDangKyDichVuKhac.IdsanPham);
            return View(goiDangKyDichVuKhac);
        }

        // GET: Admin/GoiDangKyDichVuKhacs/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.GoiDangKyDichVuKhacs.FindAsync(id);
            if (gdv != null)
            {
                _context.GoiDangKyDichVuKhacs.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool GoiDangKyDichVuKhacExists(int id)
        {
            return _context.GoiDangKyDichVuKhacs.Any(e => e.IdgoiDangKy == id);
        }
    }
}
