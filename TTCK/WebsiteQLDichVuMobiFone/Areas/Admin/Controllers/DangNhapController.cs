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
            ViewBag.ErrorMessage = TempData["Error"]; // Chuyển TempData sang ViewBag để dùng 1 lần

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.TenDangNhap == tenDangNhapHoacEmail || m.Email == tenDangNhapHoacEmail);

            if (nguoiDung != null)
            {
                if (nguoiDung.Trangthai != 1)
                {
                    TempData["Error"] = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt.";
                    return RedirectToAction("DangNhap");
                }

                if (nguoiDung.Quyen != 1)
                {
                    TempData["Error"] = "Bạn không có quyền truy cập vào hệ thống.";
                    return RedirectToAction("DangNhap");
                }

                if (_passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhau) == PasswordVerificationResult.Success)
                {
                    // Xóa session cũ trước khi đăng nhập
                    HttpContext.Session.Clear();

                    // Lưu thông tin vào session
                    HttpContext.Session.SetString("nguoidung", nguoiDung.TenDangNhap);
                    HttpContext.Session.SetString("UserId", nguoiDung.IdnguoiDung.ToString());
                    HttpContext.Session.SetString("UserAvatar", string.IsNullOrEmpty(nguoiDung.AnhDaiDien) ? "https://via.placeholder.com/150" : nguoiDung.AnhDaiDien);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Mật khẩu không chính xác.";
                    return RedirectToAction("DangNhap");
                }
            }
            else
            {
                TempData["Error"] = "Tên đăng nhập hoặc email không tồn tại.";
                return RedirectToAction("DangNhap");
            }
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ dữ liệu trong session
            return RedirectToAction("Index", "Home");
        }
    }
}
