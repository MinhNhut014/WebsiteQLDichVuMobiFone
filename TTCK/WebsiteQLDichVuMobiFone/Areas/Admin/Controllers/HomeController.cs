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
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
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
        // GET: Admin/Home
        // Trang chính của bảng điều khiển Admin
        
        public async Task<IActionResult> Index()
        {
            // Gọi GetData() nếu cần thông tin người dùng hay dữ liệu gì đó
            GetData();

            // Lấy các số liệu cần thiết
            var simCount = await _context.Sims.CountAsync();
            var mobileServiceCount = await _context.LoaiDichVuDiDongs.CountAsync();
            var otherServiceCount = await _context.LoaiDichVuKhacs.CountAsync();
            var businessServiceCount = await _context.NhomDichVuDoanhNghieps.CountAsync();

            // Lấy số lượng hóa đơn dịch vụ SIM
            var simInvoiceCount = await _context.HoaDonSims.CountAsync();

            // Lấy số lượng hóa đơn dịch vụ di động
            var mobileServiceInvoiceCount = await _context.HoaDonDichVus.CountAsync(h => h.CthoaDonDichVus.All(ct => ct.IdgoiDangKy != null));

            // Lấy số lượng hóa đơn dịch vụ khác
            var otherServiceInvoiceCount = await _context.HoaDonDichVus
                .Where(h => h.CthoaDonDichVus.Any(ct => ct.IdgoiDangKyDvk != null))
                .Where(h => h.CthoaDonDichVus.All(ct => ct.IdgoiDangKy == null))
                .CountAsync();

            // Lấy số lượng hóa đơn dịch vụ doanh nghiệp
            var businessServiceInvoiceCount = await _context.HoaDonDoanhNghieps.CountAsync();

            // Truyền dữ liệu vào ViewBag
            ViewBag.SimCount = simCount;
            ViewBag.MobileServiceCount = mobileServiceCount;
            ViewBag.OtherServiceCount = otherServiceCount;
            ViewBag.BusinessServiceCount = businessServiceCount;

            ViewBag.SimInvoiceCount = simInvoiceCount;
            ViewBag.MobileServiceInvoiceCount = mobileServiceInvoiceCount;
            ViewBag.OtherServiceInvoiceCount = otherServiceInvoiceCount;
            ViewBag.BusinessServiceInvoiceCount = businessServiceInvoiceCount;

            // Truyền dữ liệu cho biểu đồ
            ViewBag.InvoiceCounts = new int[] { simInvoiceCount, mobileServiceInvoiceCount, otherServiceInvoiceCount, businessServiceInvoiceCount };
            
            // đếm só tin liên hệ chưa xử lý 
            int soLienHeChuaXuLy = _context.LienHes.Count(x => x.TrangThai == false);
            HttpContext.Session.SetInt32("SoLienHeChuaXuLy", soLienHeChuaXuLy);
            ViewBag.SoLienHeChuaXuLy = soLienHeChuaXuLy;
            // Trả View
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            GetData();
            if (string.IsNullOrEmpty(query))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập từ khóa tìm kiếm.";
                return RedirectToAction("Index");
            }

            // Tìm kiếm trong các bảng khác nhau (ví dụ: Người dùng, SIM, Dịch vụ)
            var users = _context.NguoiDungs
                .Where(u => u.HoTen.Contains(query) || u.Email.Contains(query) || u.TenDangNhap.Contains(query))
                .ToList();

            var sims = _context.Sims
                .Where(s => s.SoThueBao.Contains(query) || s.IdloaiSoNavigation.TenLoaiSo.Contains(query))
                .Include(s => s.IddichVuNavigation)
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .Include(s => s.SimGoiDangKies)
                .ThenInclude(sg => sg.IdgoiDangKyNavigation)
                .OrderByDescending(s => s.Idsim)
                .AsQueryable()
                .ToList();

            var services = _context.GoiDangKies
                .Where(g => g.TenGoi.Contains(query) || g.IdloaiGoiNavigation.TenLoaiGoi.Contains(query))
                .Include(g => g.IdloaiGoiNavigation)
                .ThenInclude(lg => lg.IdloaiDichVuNavigation)
                .ToList();

            var spdichvukhac = _context.SanPhamDichVuKhacs
                .Where(g => g.TenSanPham.Contains(query) || g.IdloaiDichVuKhacNavigation.TenLoaiDichVu.Contains(query))
                .Include(s => s.IdloaiDichVuKhacNavigation)
                .ToList();

            var tintuc = _context.TinTucs
                .Where(g => g.TieuDe.Contains(query) || g.IdTheLoaiNavigation.TenChuDe.Contains(query))
                .Include(t => t.IdTheLoaiNavigation)
                .OrderByDescending(t => t.NgayDang)
                .ToList();

            var dichvudoanhnghiep = _context.GoiDichVus
                .Where(u => u.TenGoiDv.Contains(query) || u.IddichVuDnNavigation.TenDichVu.Contains(query))
                .Include(g => g.IddichVuDnNavigation)
                .ToList();
            var hoadondichvu = _context.HoaDonDichVus
                .Where(u => u.MaHoaDonDichVu.Contains(query))
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .ToList();
            var hoadondoanhnghiep = _context.HoaDonDoanhNghieps
                .Where(u => u.MaHoaDonDoanhNghiep.Contains(query))
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .ToList();

            var hoadonsim = _context.HoaDonSims
                .Where(u => u.MaHoaDonSim.Contains(query))
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdphuongThucVcNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .OrderByDescending(h => h.NgayDatHang)
                .ToList();
            var hoadonnap = _context.GiaoDichNapTiens
                .Where(u => u.MaGiaoDichNapTien.Contains(query))
                .Include(g => g.IdnguoiDungNavigation).Include(g => g.IdsimNavigation).Include(g => g.IdtrangThaiThanhToanNavigation)
                .ToList();
            var trangthaisim = await _context.TrangThaiSims.ToListAsync();
            // Truyền tất cả kết quả tìm kiếm vào ViewBag
            ViewBag.Users = users;
            ViewBag.Sims = sims;
            ViewBag.Services = services;
            ViewBag.SPDichVuKhac = spdichvukhac;
            ViewBag.TinTuc = tintuc;
            ViewBag.DichVuDoanhNghiep = dichvudoanhnghiep;
            ViewBag.HoaDonDichVu = hoadondichvu;
            ViewBag.HoaDonDoanhNghiep = hoadondoanhnghiep;
            ViewBag.HoaDonSim = hoadonsim;
            ViewBag.HoaDonNap = hoadonnap;
            ViewBag.Query = query;

            ViewBag.TrangThaiSim = trangthaisim;
            ViewBag.TrangThaiDonHang = await _context.TrangThaiDonHangs.ToListAsync();
            ViewBag.TrangThaiThanhToan = await _context.TrangThaiThanhToans.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole(int id, int quyen)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            // Giới hạn giá trị quyền (1 = Admin, 0 = Người dùng)
            user.Quyen = (quyen == 0 || quyen == 1) ? quyen : user.Quyen;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusHDDN(int id, int trangthai)
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
        [HttpPost]
        public async Task<IActionResult> UpdateStatusHDDV(int id, int trangthai)
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
        [HttpPost]
        public async Task<IActionResult> UpdateStatusUser(int id, int trangthai)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            // Giới hạn trạng thái (1 = Hoạt động, 0 = Bị khóa)
            user.Trangthai = (trangthai == 0 || trangthai == 1) ? trangthai : user.Trangthai;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusSim(int id, int trangthai)
        {
            var sim = await _context.Sims.FindAsync(id);
            if (sim == null) return NotFound();

            var trangThaiTonTai = await _context.TrangThaiSims.AnyAsync(t => t.IdtrangThaiSim == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            sim.IdtrangThaiSim = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái SIM thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatusHDSim(int id, int trangthai)
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
        public async Task<IActionResult> UpdateStatusThanhToanSim(int id, int trangthai)
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
        [HttpPost]
        public async Task<IActionResult> UpdateStatusThanhToanNapTien(int id, int trangthai)
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
        // GET: Admin/Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.IdnguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: Admin/Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdnguoiDung,AnhDaiDien,HoTen,Cccd,Email,SoDienThoai,DiaChi,TenDangNhap,MatKhau")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // GET: Admin/Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(nguoiDung);
        }

        // POST: Admin/Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdnguoiDung,AnhDaiDien,HoTen,Cccd,Email,SoDienThoai,DiaChi,TenDangNhap,MatKhau")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.IdnguoiDung)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.IdnguoiDung))
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
            return View(nguoiDung);
        }

        // GET: Admin/Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.IdnguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: Admin/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.IdnguoiDung == id);
        }
    }
}
