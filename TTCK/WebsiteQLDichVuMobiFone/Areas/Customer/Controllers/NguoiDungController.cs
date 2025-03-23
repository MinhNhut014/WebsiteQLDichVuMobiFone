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

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<NguoiDung> _passwordHasher;

        public NguoiDungController(ApplicationDbContext context, IPasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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
            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.TenDangNhap == tenDangNhapHoacEmail || m.Email == tenDangNhapHoacEmail);

            if (nguoiDung != null)
            {
                if (_passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhau) == PasswordVerificationResult.Success)
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.SetString("nguoidung", nguoiDung.TenDangNhap);
                    HttpContext.Session.SetString("UserId", nguoiDung.IdnguoiDung.ToString());
                    HttpContext.Session.SetString("UserAvatar", string.IsNullOrEmpty(nguoiDung.AnhDaiDien) ? "https://via.placeholder.com/150" : nguoiDung.AnhDaiDien);

                    // Chuyển hướng người dùng đến trang chính
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    TempData["Error"] = "Mật khẩu không chính xác.";
                }
            }
            else
            {
                TempData["Error"] = "Tên đăng nhập hoặc email không tồn tại.";
            }

            return View();
        }
        // Logout
        public IActionResult DangXuat()
        {
            HttpContext.Session.SetString("nguoidung", string.Empty);
            return RedirectToAction("Index", "Home");
        }

        // Register
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKy(string hoTen, string soDienThoai, string email, string tenDangNhap, string cccd, string matKhau, string diaChi)
        {
            try
            {
                // Kiểm tra trùng lặp dữ liệu
                try
                {
                    var tonTaiNguoiDung = await _context.NguoiDungs
                                .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap || x.SoDienThoai == soDienThoai || x.Email == email);

                    if (tonTaiNguoiDung != null)
                    {
                        string loi = "Thông tin đã được sử dụng: ";
                        if (tonTaiNguoiDung.TenDangNhap == tenDangNhap) loi += "Tên đăng nhập, ";
                        if (tonTaiNguoiDung.SoDienThoai == soDienThoai) loi += "Số điện thoại, ";
                        if (tonTaiNguoiDung.Email == email) loi += "Email, ";

                        TempData["Error"] = loi.TrimEnd(',', ' ');
                        return View();
                    }

                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi kiểm tra dữ liệu trùng lặp: {ex.Message}";
                    return View();
                }

                // Tạo người dùng mới
                var nguoiDung = new NguoiDung
                {
                    HoTen = hoTen,
                    SoDienThoai = soDienThoai,
                    Email = email,
                    AnhDaiDien = "user.jpg",
                    Cccd = cccd,
                    TenDangNhap = tenDangNhap,
                    MatKhau = _passwordHasher.HashPassword(null, matKhau),
                    DiaChi = diaChi
                };

                // Lưu vào cơ sở dữ liệu
                try
                {
                    _context.Add(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi lưu dữ liệu vào cơ sở dữ liệu: {ex.Message}";
                    return View();
                }

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi không xác định: {ex.Message}";
                return View();
            }
        }

        // Change Password
        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["Error"] = "Bạn cần đăng nhập để thay đổi mật khẩu.";
                return RedirectToAction("DangNhap");
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);
            if (nguoiDung == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("DangNhap");
            }

            var result = _passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhauCu);
            if (result != PasswordVerificationResult.Success)
            {
                TempData["Error"] = "Mật khẩu cũ không đúng.";
                return View();
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return View();
            }

            nguoiDung.MatKhau = _passwordHasher.HashPassword(nguoiDung, matKhauMoi);
            _context.Update(nguoiDung);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Mật khẩu đã được thay đổi thành công.";
            return RedirectToAction("HoSoNguoiDung");
        }

        // Profile (HoSoNguoiDung)
        public IActionResult HoSoNguoiDung()
        {
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["Error"] = "Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }

            var nguoiDung = _context.NguoiDungs.FirstOrDefault(x => x.TenDangNhap == tenDangNhap);
            if (nguoiDung == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("DangNhap");
            }

            return View(nguoiDung);
        }
    }
}
