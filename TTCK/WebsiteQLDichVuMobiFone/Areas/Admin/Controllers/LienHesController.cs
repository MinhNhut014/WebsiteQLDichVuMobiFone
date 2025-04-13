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
    public class LienHesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LienHesController(ApplicationDbContext context)
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
        // GET: Admin/LienHes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LienHes.Include(l => l.IdnguoiDungNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        // POST: Cập nhật trạng thái người dùng
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, bool? trangthai)
        {
            var lienhe = await _context.LienHes.FindAsync(id);
            if (lienhe == null) return NotFound();

            // Giới hạn trạng thái (1 = Hoạt động, 0 = Bị khóa)
            lienhe.TrangThai = trangthai == false || trangthai == true ? trangthai : lienhe.TrangThai;
            await _context.SaveChangesAsync();
            // Cập nhật lại Session sau khi thay đổi trạng thái
            int soLienHeChuaXuLy = await _context.LienHes.CountAsync(x => x.TrangThai == false);
            HttpContext.Session.SetInt32("SoLienHeChuaXuLy", soLienHeChuaXuLy);
            return RedirectToAction("Index");
        }
        // GET: Admin/LienHes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lienHe = await _context.LienHes
                .Include(l => l.IdnguoiDungNavigation)
                .FirstOrDefaultAsync(m => m.IdlienHe == id);
            if (lienHe == null)
            {
                return NotFound();
            }

            return View(lienHe);
        }

        // GET: Admin/LienHes/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "HoTen");
            return View();
        }

        // POST: Admin/LienHes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdlienHe,IdnguoiDung,HoTen,Email,SoDienThoai,NoiDung,NgayGui,TrangThai")] LienHe lienHe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lienHe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "HoTen", lienHe.IdnguoiDung);
            return View(lienHe);
        }

        // GET: Admin/LienHes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lienHe = await _context.LienHes.FindAsync(id);
            if (lienHe == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "HoTen", lienHe.IdnguoiDung);
            return View(lienHe);
        }

        // POST: Admin/LienHes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdlienHe,IdnguoiDung,HoTen,Email,SoDienThoai,NoiDung,NgayGui,TrangThai")] LienHe lienHe)
        {
            if (id != lienHe.IdlienHe)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lienHe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LienHeExists(lienHe.IdlienHe))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "HoTen", lienHe.IdnguoiDung);
            return View(lienHe);
        }

        // GET: Admin/LienHes/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            GetData();
            var lienHe = await _context.LienHes.FindAsync(id);
            if (lienHe != null)
            {
                _context.LienHes.Remove(lienHe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool LienHeExists(int id)
        {
            return _context.LienHes.Any(e => e.IdlienHe == id);
        }
    }
}
