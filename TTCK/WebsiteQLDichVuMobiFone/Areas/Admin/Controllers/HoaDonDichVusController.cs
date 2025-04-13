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
    public class HoaDonDichVusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonDichVusController(ApplicationDbContext context)
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

        // GET: Admin/HoaDonDichVus
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            GetData();

            // Lấy danh sách ngày đặt hàng duy nhất
            var ngayDatHangList = await _context.HoaDonDichVus
                .Where(h => h.NgayDatHang.HasValue)
                .Select(h => h.NgayDatHang.Value.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToListAsync();

            ViewData["NgayDatHangList"] = ngayDatHangList; // Gửi danh sách ngày đặt hàng qua View

            // Truy vấn hóa đơn có lọc theo ngày
            var query = _context.HoaDonDichVus
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .Where(h => h.CthoaDonDichVus.Any(ct => ct.IdgoiDangKy != null))
                .Where(h => h.CthoaDonDichVus.All(ct => ct.IdgoiDangKyDvk == null))
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

        // POST: Cập nhật trạng thái hóa đơn sim
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangthai)
        {
            var hoadon = await _context.HoaDonDichVus.FindAsync(id);
            if (hoadon == null) return NotFound();

            // Kiểm tra xem trạng thái có tồn tại trong bảng TrangThaiDonHang không
            var trangThaiTonTai = await _context.TrangThaiDonHangs.AnyAsync(t => t.IdtrangThai == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            hoadon.IdtrangThai = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/HoaDonDichVus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            GetData();
            var hoaDon = await _context.HoaDonDichVus
                                .Include(x => x.CthoaDonDichVus)
                                    .ThenInclude(x => x.IdgoiDangKyNavigation)
                                .Include(x => x.CthoaDonDichVus)
                                    .ThenInclude(x => x.IdgoiDangKyDvkNavigation)
                                .Include(x => x.IdnguoiDungNavigation) // Thêm thông tin người dùng
                                .FirstOrDefaultAsync(x => x.IdhoaDonDv == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: Admin/HoaDonDichVus/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai");
            return View();
        }

        // POST: Admin/HoaDonDichVus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdhoaDonDv,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email")] HoaDonDichVu hoaDonDichVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonDichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDichVu.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDichVu.IdtrangThai);
            return View(hoaDonDichVu);
        }

        // GET: Admin/HoaDonDichVus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDichVu = await _context.HoaDonDichVus.FindAsync(id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDichVu.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDichVu.IdtrangThai);
            return View(hoaDonDichVu);
        }

        // POST: Admin/HoaDonDichVus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdhoaDonDv,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email")] HoaDonDichVu hoaDonDichVu)
        {
            if (id != hoaDonDichVu.IdhoaDonDv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonDichVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonDichVuExists(hoaDonDichVu.IdhoaDonDv))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDichVu.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDichVu.IdtrangThai);
            return View(hoaDonDichVu);
        }

        // GET: Admin/HoaDonDichVus/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoadon = await _context.HoaDonDichVus.FindAsync(id);
            if (hoadon != null)
            {
                _context.HoaDonDichVus.Remove(hoadon);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool HoaDonDichVuExists(int id)
        {
            return _context.HoaDonDichVus.Any(e => e.IdhoaDonDv == id);
        }
    }
}
