using System;
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
    public class HoaDonDoanhNghiepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonDoanhNghiepsController(ApplicationDbContext context)
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

        // GET: Admin/HoaDonDoanhNghieps
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            GetData();

            // Lấy danh sách ngày đặt hàng duy nhất
            var ngayDatHangList = await _context.HoaDonDoanhNghieps
                .Where(h => h.NgayDatHang.HasValue)
                .Select(h => h.NgayDatHang.Value.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToListAsync();

            ViewData["NgayDatHangList"] = ngayDatHangList; // Gửi danh sách ngày đặt hàng qua View

            // Truy vấn hóa đơn có lọc theo ngày
            var query = _context.HoaDonDoanhNghieps
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .AsQueryable();

            // Lọc theo khoảng ngày nếu người dùng chọn
            if (fromDate.HasValue && toDate.HasValue)
            {
                DateTime from = fromDate.Value.Date;
                DateTime to = toDate.Value.Date.AddDays(1).AddTicks(-1); // Lấy hết ngày cuối cùng

                query = query.Where(h => h.NgayDatHang >= from && h.NgayDatHang <= to);
            }

            ViewBag.TrangThaiDonHang = await _context.TrangThaiDonHangs
                .Where(tt => tt.IdtrangThai != 2)
                .ToListAsync();

            return View(await query.OrderByDescending(h => h.NgayDatHang).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangthai)
        {
            var hoadon = await _context.HoaDonDoanhNghieps.FindAsync(id);
            if (hoadon == null) return NotFound();

            // Kiểm tra xem trạng thái có tồn tại trong bảng TrangThaiDonHang không
            var trangThaiTonTai = await _context.TrangThaiDonHangs.AnyAsync(t => t.IdtrangThai == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            hoadon.IdtrangThai = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/HoaDonDoanhNghieps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDoanhNghiep = await _context.HoaDonDoanhNghieps
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonDn == id);
            if (hoaDonDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(hoaDonDoanhNghiep);
        }

        // GET: Admin/HoaDonDoanhNghieps/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai");
            return View();
        }

        // POST: Admin/HoaDonDoanhNghieps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdhoaDonDn,IdnguoiDung,NgayDatHang,IdtrangThai,TenCongTy,SoDienThoaiCongTy,EmailCongTy,DiaChiCongTy")] HoaDonDoanhNghiep hoaDonDoanhNghiep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonDoanhNghiep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDoanhNghiep.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDoanhNghiep.IdtrangThai);
            return View(hoaDonDoanhNghiep);
        }

        // GET: Admin/HoaDonDoanhNghieps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDoanhNghiep = await _context.HoaDonDoanhNghieps.FindAsync(id);
            if (hoaDonDoanhNghiep == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDoanhNghiep.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDoanhNghiep.IdtrangThai);
            return View(hoaDonDoanhNghiep);
        }

        // POST: Admin/HoaDonDoanhNghieps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdhoaDonDn,IdnguoiDung,NgayDatHang,IdtrangThai,TenCongTy,SoDienThoaiCongTy,EmailCongTy,DiaChiCongTy")] HoaDonDoanhNghiep hoaDonDoanhNghiep)
        {
            if (id != hoaDonDoanhNghiep.IdhoaDonDn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonDoanhNghiep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonDoanhNghiepExists(hoaDonDoanhNghiep.IdhoaDonDn))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDoanhNghiep.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDoanhNghiep.IdtrangThai);
            return View(hoaDonDoanhNghiep);
        }

        // GET: Admin/HoaDonDoanhNghieps/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hddoanhnghiep = await _context.HoaDonDoanhNghieps.FindAsync(id);
            if (hddoanhnghiep != null)
            {
                _context.HoaDonDoanhNghieps.Remove(hddoanhnghiep);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool HoaDonDoanhNghiepExists(int id)
        {
            return _context.HoaDonDoanhNghieps.Any(e => e.IdhoaDonDn == id);
        }
    }
}
