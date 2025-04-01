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
    public class LoaiDichVuKhacsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiDichVuKhacsController(ApplicationDbContext context)
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
                .Include(l => l.SanPhamDichVuKhacs)
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
            if (await _context.LoaiDichVuKhacs.AnyAsync(g => g.TenLoaiDichVu == loaiDichVuKhac.TenLoaiDichVu))
            {
                ModelState.AddModelError("TenLoaiDichVu", "Dịch vụ này đã có rồi, vui lòng nhập tên dịch vụ khác.");
            }
            // Gán ID Dịch Vụ cố định cho loại dịch vụ di động
            loaiDichVuKhac.IddichVu = 4;
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
                    loaiDichVuKhac.IddichVu = 4;
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
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.LoaiDichVuKhacs.FindAsync(id);
            if (gdv != null)
            {
                _context.LoaiDichVuKhacs.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool LoaiDichVuKhacExists(int id)
        {
            return _context.LoaiDichVuKhacs.Any(e => e.IdloaiDichVuKhac == id);
        }
    }
}
