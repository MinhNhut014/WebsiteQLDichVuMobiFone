using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DangNhapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<NguoiDung> _passwordHasher;

        public DangNhapController(ApplicationDbContext context, IPasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public IActionResult Index()
        {
            return View();
        }
        // Login
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
        public IActionResult DangNhap()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(string tenDangNhapHoacEmail, string matKhau)
        {
            // Không sử dụng TempData mà chuyển thẳng lỗi vào ViewBag
            ViewBag.ErrorMessage = null;

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.TenDangNhap == tenDangNhapHoacEmail || m.Email == tenDangNhapHoacEmail);

            if (nguoiDung == null)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc email không tồn tại.";
                return View();
            }

            if (nguoiDung.Trangthai != 1)
            {
                ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt.";
                return View();
            }

            if (nguoiDung.Quyen != 1)
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập vào hệ thống.";
                return View();
            }

            if (_passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhau) != PasswordVerificationResult.Success)
            {
                ViewBag.ErrorMessage = "Mật khẩu không chính xác.";
                return View();
            }

            // Xóa session cũ trước khi đăng nhập
            HttpContext.Session.Clear();

            // Lưu thông tin vào session
            HttpContext.Session.SetString("nguoidung", nguoiDung.TenDangNhap);
            HttpContext.Session.SetString("UserId", nguoiDung.IdnguoiDung.ToString());
            HttpContext.Session.SetString("UserAvatar", string.IsNullOrEmpty(nguoiDung.AnhDaiDien) ? "https://via.placeholder.com/150" : nguoiDung.AnhDaiDien);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ dữ liệu trong session
            return RedirectToAction("Index", "Home");
        }
    }
}
