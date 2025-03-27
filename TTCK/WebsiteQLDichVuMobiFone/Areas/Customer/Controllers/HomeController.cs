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
