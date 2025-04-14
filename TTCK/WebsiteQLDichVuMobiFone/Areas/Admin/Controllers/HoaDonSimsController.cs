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
    public class HoaDonSimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonSimsController(ApplicationDbContext context)
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
        // GET: Admin/HoaDonSims
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var ngayDatHangList = await _context.HoaDonSims
                .Where(h => h.NgayDatHang.HasValue) // Lọc bỏ giá trị null
                .Select(h => h.NgayDatHang.Value.Date) // Chỉ lấy phần ngày (không lấy giờ)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            ViewData["NgayDatHangList"] = ngayDatHangList; // Truyền danh sách ngày vào View

            // Truy vấn danh sách hóa đơn SIM
            var query = _context.HoaDonSims
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdphuongThucVcNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .OrderByDescending(h => h.NgayDatHang) // Sắp xếp theo ngày đặt hàng mới nhất
                .AsQueryable();

            // Lọc theo khoảng ngày nếu được chọn
            if (fromDate.HasValue && toDate.HasValue)
            {
                // Lấy khoảng từ 00:00:00 đến 23:59:59 để tránh lỗi lọc sai
                DateTime from = fromDate.Value.Date;
                DateTime to = toDate.Value.Date.AddDays(1).AddTicks(-1);

                query = query.Where(h => h.NgayDatHang >= from && h.NgayDatHang <= to);
            }
            // Truy vấn danh sách trạng thái đơn hàng
            ViewBag.TrangThaiDonHang = await _context.TrangThaiDonHangs.ToListAsync();
            ViewBag.TrangThaiThanhToan = await _context.TrangThaiThanhToans.ToListAsync();
            return View(await query.ToListAsync());
        }

        // POST: Cập nhật trạng thái hóa đơn sim
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangthai)
        {
            var hdsim = await _context.HoaDonSims.FindAsync(id);
            if (hdsim == null) return NotFound();

            // Kiểm tra xem trạng thái có tồn tại trong bảng TrangThaiDonHang không
            var trangThaiTonTai = await _context.TrangThaiDonHangs.AnyAsync(t => t.IdtrangThai == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            hdsim.IdtrangThai = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusThanhToan(int id, int trangthai)
        {
            var hdsim = await _context.HoaDonSims.FindAsync(id);
            if (hdsim == null) return NotFound();

            // Kiểm tra xem trạng thái có tồn tại trong bảng TrangThaiDonHang không
            var trangThaiTonTai = await _context.TrangThaiThanhToans.AnyAsync(t => t.IdtrangThaiThanhToan == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            hdsim.IdtrangThaiThanhToan = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/HoaDonSims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            GetData();
            var hoaDonSim = await _context.HoaDonSims
                .Include(x => x.CthoaDonSims)
                    .ThenInclude(x => x.IdgoiDangKyNavigation) // Lấy thông tin gói đăng ký
                .Include(x => x.CthoaDonSims)
                    .ThenInclude(x => x.IdsimNavigation) // Lấy thông tin SIM
                        .ThenInclude(s => s.IdloaiSoNavigation) // Lấy thông tin loại SIM
                .Include(x => x.CthoaDonSims)
                    .ThenInclude(x => x.IdsimNavigation)
                        .ThenInclude(s => s.IdtrangThaiSimNavigation) // Lấy thông tin trạng thái SIM
                .FirstOrDefaultAsync(x => x.IdhoaDonSim == id);

            if (hoaDonSim == null)
            {
                return NotFound();
            }

            return View(hoaDonSim);
        }

        // GET: Admin/HoaDonSims/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc");
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai");
            return View();
        }

        // POST: Admin/HoaDonSims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdhoaDonSim,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email,DiaDiemNhan,PhuongThucThanhToan,IdphuongThucVc")] HoaDonSim hoaDonSim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonSim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonSim.IdnguoiDung);
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc", hoaDonSim.IdphuongThucVc);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonSim.IdtrangThai);
            return View(hoaDonSim);
        }

        // GET: Admin/HoaDonSims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonSim = await _context.HoaDonSims.FindAsync(id);
            if (hoaDonSim == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonSim.IdnguoiDung);
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc", hoaDonSim.IdphuongThucVc);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonSim.IdtrangThai);
            return View(hoaDonSim);
        }

        // POST: Admin/HoaDonSims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdhoaDonSim,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email,DiaDiemNhan,PhuongThucThanhToan,IdphuongThucVc")] HoaDonSim hoaDonSim)
        {
            if (id != hoaDonSim.IdhoaDonSim)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonSim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonSimExists(hoaDonSim.IdhoaDonSim))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonSim.IdnguoiDung);
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc", hoaDonSim.IdphuongThucVc);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonSim.IdtrangThai);
            return View(hoaDonSim);
        }

        // GET: Admin/HoaDonSims/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hdsim = await _context.HoaDonSims.FindAsync(id);
            if (hdsim != null)
            {
                _context.HoaDonSims.Remove(hdsim);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool HoaDonSimExists(int id)
        {
            return _context.HoaDonSims.Any(e => e.IdhoaDonSim == id);
        }
    }
}
