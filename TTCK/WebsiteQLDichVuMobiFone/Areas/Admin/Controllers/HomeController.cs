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

            // Trả View
            return View();
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
