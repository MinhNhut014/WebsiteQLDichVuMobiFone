using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Areas.Customer.ViewModels;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
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

        public IActionResult Index()
        {
            GetData();
            var danhSachDichVu = _context.DichVus.ToList();
            // Dictionary ánh xạ tên dịch vụ với icon
            var iconDichVu = new Dictionary<string, string>
            {
                { "SIM", "fa-sim-card" }, // Icon thẻ SIM
                { "Dịch vụ di động", "fa-mobile-alt" }, // Icon điện thoại di động
                { "Dịch vụ doanh nghiệp", "fa-briefcase" }, // Icon cặp tài liệu (liên quan đến doanh nghiệp)
                { "Dịch vụ khác", "fa-cogs" } // Icon bánh răng (biểu thị các dịch vụ khác)
            };
            
            ViewBag.DanhSachDichVu = danhSachDichVu;
            ViewBag.IconDichVu = iconDichVu;
            ViewBag.DanhSachTinTuc = _context.TinTucs.ToList();
            return View();
        }
        public IActionResult TimKiem(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View();
            }

            // Tìm kiếm gói đăng ký
            var goiDangKyResults = _context.GoiDangKies
                .Where(g => g.TenGoi.Contains(searchTerm))
                .ToList();

            // Tìm kiếm sản phẩm dịch vụ khác
            var sanPhamDichVuResults = _context.SanPhamDichVuKhacs
                .Where(s => s.TenSanPham.Contains(searchTerm))
                .ToList();

            // Tìm kiếm gói dịch vụ
            var goiDichVuResults = _context.GoiDichVus
                .Where(g => g.TenGoiDv.Contains(searchTerm))
                .ToList();

            // Tìm kiếm tin tức
            var tinTucResults = _context.TinTucs
                .Where(t => t.TieuDe.Contains(searchTerm) || t.NoiDung.Contains(searchTerm))
                .ToList();

            // Chuyển dữ liệu vào ViewBag
            ViewBag.GoiDangKy = goiDangKyResults;
            ViewBag.SanPhamDichVu = sanPhamDichVuResults;
            ViewBag.GoiDichVu = goiDichVuResults;
            ViewBag.TinTuc = tinTucResults;

            return View();
        }
        public IActionResult LienHe()
        {
            GetData();
            return View();
        }
        public IActionResult GioiThieu()
        {
            GetData();
            return View();
        }
        public IActionResult LichSu()
        {
            GetData();
            return View();
        }
        public IActionResult CamKet()
        {
            GetData();
            return View();
        }
        public IActionResult CoCau()
        {
            GetData();
            return View();
        }
        public IActionResult QuanLy()
        {
            GetData();
            return View();
        }
        public IActionResult TamNhin()
        {
            GetData();
            return View();
        }
        public IActionResult CongKhai()
        {
            GetData();
            return View();
        }
    }
}
