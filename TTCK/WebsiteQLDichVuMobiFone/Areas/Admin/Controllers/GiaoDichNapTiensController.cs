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
    public class GiaoDichNapTiensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiaoDichNapTiensController(ApplicationDbContext context)
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
        // GET: Admin/GiaoDichNapTiens
        public async Task<IActionResult> Index()
        {
            ViewBag.TrangThaiThanhToan = await _context.TrangThaiThanhToans.ToListAsync();
            var applicationDbContext = _context.GiaoDichNapTiens.Include(g => g.IdnguoiDungNavigation).Include(g => g.IdsimNavigation).Include(g => g.IdtrangThaiThanhToanNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/GiaoDichNapTiens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoDichNapTien = await _context.GiaoDichNapTiens
                .Include(g => g.IdnguoiDungNavigation)
                .Include(g => g.IdsimNavigation)
                .Include(g => g.IdtrangThaiThanhToanNavigation)
                .FirstOrDefaultAsync(m => m.IdgiaoDich == id);
            if (giaoDichNapTien == null)
            {
                return NotFound();
            }

            return View(giaoDichNapTien);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusThanhToan(int id, int trangthai)
        {
            var hdsim = await _context.GiaoDichNapTiens.FindAsync(id);
            if (hdsim == null) return NotFound();

            // Kiểm tra xem trạng thái có tồn tại trong bảng TrangThaiDonHang không
            var trangThaiTonTai = await _context.TrangThaiThanhToans.AnyAsync(t => t.IdtrangThaiThanhToan == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            hdsim.IdtrangThaiThanhToan = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }
        // GET: Admin/GiaoDichNapTiens/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim");
            ViewData["IdtrangThaiThanhToan"] = new SelectList(_context.TrangThaiThanhToans, "IdtrangThaiThanhToan", "IdtrangThaiThanhToan");
            return View();
        }

        // POST: Admin/GiaoDichNapTiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdgiaoDich,MaGiaoDichNapTien,Idsim,IdnguoiDung,SoTienNap,NgayNap,GhiChu,IdtrangThaiThanhToan,PhuongThucNap")] GiaoDichNapTien giaoDichNapTien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giaoDichNapTien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", giaoDichNapTien.IdnguoiDung);
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim", giaoDichNapTien.Idsim);
            ViewData["IdtrangThaiThanhToan"] = new SelectList(_context.TrangThaiThanhToans, "IdtrangThaiThanhToan", "IdtrangThaiThanhToan", giaoDichNapTien.IdtrangThaiThanhToan);
            return View(giaoDichNapTien);
        }

        // GET: Admin/GiaoDichNapTiens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giaoDichNapTien = await _context.GiaoDichNapTiens.FindAsync(id);
            if (giaoDichNapTien == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", giaoDichNapTien.IdnguoiDung);
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim", giaoDichNapTien.Idsim);
            ViewData["IdtrangThaiThanhToan"] = new SelectList(_context.TrangThaiThanhToans, "IdtrangThaiThanhToan", "IdtrangThaiThanhToan", giaoDichNapTien.IdtrangThaiThanhToan);
            return View(giaoDichNapTien);
        }

        // POST: Admin/GiaoDichNapTiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdgiaoDich,MaGiaoDichNapTien,Idsim,IdnguoiDung,SoTienNap,NgayNap,GhiChu,IdtrangThaiThanhToan,PhuongThucNap")] GiaoDichNapTien giaoDichNapTien)
        {
            if (id != giaoDichNapTien.IdgiaoDich)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaoDichNapTien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiaoDichNapTienExists(giaoDichNapTien.IdgiaoDich))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", giaoDichNapTien.IdnguoiDung);
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim", giaoDichNapTien.Idsim);
            ViewData["IdtrangThaiThanhToan"] = new SelectList(_context.TrangThaiThanhToans, "IdtrangThaiThanhToan", "IdtrangThaiThanhToan", giaoDichNapTien.IdtrangThaiThanhToan);
            return View(giaoDichNapTien);
        }

        // GET: Admin/GiaoDichNapTiens/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hdsim = await _context.GiaoDichNapTiens.FindAsync(id);
            if (hdsim != null)
            {
                _context.GiaoDichNapTiens.Remove(hdsim);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GiaoDichNapTienExists(int id)
        {
            return _context.GiaoDichNapTiens.Any(e => e.IdgiaoDich == id);
        }
    }
}
