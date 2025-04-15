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
using WebsiteQLDichVuMobiFone.Services.Mail;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<NguoiDung> _passwordHasher;

        public NguoiDungController(ApplicationDbContext context, IEmailService emailService, IPasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _emailService = emailService;
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

        public IActionResult DangNhap(string returnUrl = null)
        {
            GetData();
            ViewBag.ReturnUrl = returnUrl; // Lưu URL trước đó để chuyển hướng sau đăng nhập
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(string tenDangNhapHoacEmail, string matKhau, string returnUrl = null)
        {
            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.TenDangNhap == tenDangNhapHoacEmail || m.Email == tenDangNhapHoacEmail);

            if (nguoiDung != null)
            {
                if (nguoiDung.Trangthai != 1)
                {
                    ViewBag.ErrorMessage = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt.";
                    return View();
                }

                if (_passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhau) == PasswordVerificationResult.Success)
                {
                    // Lưu thông tin vào session
                    HttpContext.Session.SetString("nguoidung", nguoiDung.TenDangNhap);
                    HttpContext.Session.SetString("UserId", nguoiDung.IdnguoiDung.ToString());
                    HttpContext.Session.SetString("UserAvatar", string.IsNullOrEmpty(nguoiDung.AnhDaiDien) ? "https://via.placeholder.com/150" : nguoiDung.AnhDaiDien);

                    // Kiểm tra returnUrl và điều hướng về trang trước đó nếu có
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Mật khẩu không chính xác.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc email không tồn tại.";
            }

            return View();
        }
        // Logout
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ dữ liệu trong session
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
                TempData["Loi"] = "Bạn cần đăng nhập để thay đổi mật khẩu.";
                return RedirectToAction("DangNhap");
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);
            if (nguoiDung == null)
            {
                TempData["Loi"] = "Không tìm thấy tài khoản người dùng.";
                return RedirectToAction("DangNhap");
            }

            if (_passwordHasher.VerifyHashedPassword(nguoiDung, nguoiDung.MatKhau, matKhauCu) != PasswordVerificationResult.Success)
            {
                TempData["Loi"] = "Mật khẩu cũ không đúng.";
                return View();
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                TempData["Loi"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return View();
            }

            nguoiDung.MatKhau = _passwordHasher.HashPassword(nguoiDung, matKhauMoi);
            _context.Update(nguoiDung);
            await _context.SaveChangesAsync();
            TempData["ThanhCong"] = "Mật khẩu đã được thay đổi thành công.";
            return RedirectToAction("HoSoNguoiDung");
        }

        [HttpPost]
        public IActionResult QuenMatKhau(string tenDangNhap)
        {
            var user = _context.NguoiDungs.FirstOrDefault(x => x.TenDangNhap == tenDangNhap || x.Email == tenDangNhap);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("DangNhap");
            }

            // Tạo mã OTP và lưu vào Session
            string otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("NguoiDungID", user.IdnguoiDung.ToString());
            HttpContext.Session.SetString("TenDangNhapOTP", tenDangNhap);

            // Gửi OTP qua email hoặc SMS (bạn cần có logic ở đây)
            _emailService.SendOTP(user.Email, otp);

            // Chuyển sang trang xác nhận mã OTP
            return RedirectToAction("XacNhanOTP");
        }

        [HttpGet]
        public IActionResult XacNhanOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KiemTraOTP(string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var tenDangNhap = HttpContext.Session.GetString("TenDangNhapOTP");

            if (string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ErrorMessage"] = "OTP đã hết hạn hoặc không hợp lệ.";
                return RedirectToAction("DangNhap");
            }

            if (otp != sessionOtp)
            {
                TempData["ErrorMessage"] = "Mã OTP không đúng.";
                return RedirectToAction("XacNhanOTP");
            }

            // Xóa OTP khỏi Session
            HttpContext.Session.Remove("OTP");

            return RedirectToAction("NhapMatKhauMoi");
        }
        [HttpGet]
        public IActionResult NhapMatKhauMoi()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LuuMatKhauMoi(string matKhauMoi, string xacNhanMatKhau)
        {
            var tenDangNhap = HttpContext.Session.GetString("TenDangNhapOTP");

            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn. Vui lòng thử lại.";
                return RedirectToAction("DangNhap");
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return RedirectToAction("NhapMatKhauMoi");
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(nd => nd.TenDangNhap == tenDangNhap);
            if (nguoiDung == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("DangNhap");
            }

            // Cập nhật mật khẩu mới
            nguoiDung.MatKhau = _passwordHasher.HashPassword(nguoiDung, matKhauMoi);
            _context.Update(nguoiDung);
            await _context.SaveChangesAsync();

            // Xóa thông tin khỏi Session
            HttpContext.Session.Remove("TenDangNhapOTP");

            TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";
            return RedirectToAction("DangNhap");
        }

        private async Task GuiEmail(string email, string subject, string body)
        {
            using var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("nhut_dth216071@student.agu.edu.vn", "dmNhut.0312.01042k3.mail"),
                EnableSsl = true,
            };

            var message = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress("nhut_dth216071@student.agu.edu.vnm", "MobiFone Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            message.To.Add(email);

            await smtp.SendMailAsync(message);
        }


        public IActionResult HoSoNguoiDung(string section = "hoso")
        {
            GetData();
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["Loi"] = "Vui lòng đăng nhập để xem hồ sơ cá nhân.";
                return RedirectToAction("DangNhap");
            }

            var nguoiDung = _context.NguoiDungs
                    .Include(nd => nd.GiaoDichNapTiens.OrderByDescending(gd => gd.NgayNap)) // Lấy lịch sử nạp tiền
                        .ThenInclude(nd => nd.IdtrangThaiThanhToanNavigation)
                    .Include(nd => nd.Sims)
                        .ThenInclude(sim => sim.IdloaiSoNavigation)
                    .Include(nd => nd.Sims)
                        .ThenInclude(sim => sim.IdtrangThaiSimNavigation)
                     .Include(x => x.HoaDonDichVus.OrderByDescending(hd => hd.NgayDatHang))
                         .ThenInclude(hd => hd.IdtrangThaiNavigation)
                     .Include(x => x.HoaDonDichVus.OrderByDescending(hd => hd.NgayDatHang))
                         .ThenInclude(hd => hd.CthoaDonDichVus)
                     .Include(x => x.HoaDonDoanhNghieps.OrderByDescending(hd => hd.NgayDatHang))
                         .ThenInclude(hd => hd.IdtrangThaiNavigation)
                     .Include(x => x.HoaDonSims.OrderByDescending(hd => hd.NgayDatHang))
                         .ThenInclude(hd => hd.IdtrangThaiNavigation)
                     .FirstOrDefault(x => x.TenDangNhap == tenDangNhap);


            if (nguoiDung == null)
            {
                TempData["Loi"] = "Không tìm thấy tài khoản người dùng.";
                return RedirectToAction("DangNhap");
            }

            if (nguoiDung.Trangthai == 0) // Kiểm tra tài khoản có bị khóa không
            {
                TempData["Loi"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.";
                return RedirectToAction("DangNhap");
            }

            // Khởi tạo danh sách rỗng nếu cần thiết để tránh lỗi null reference
            nguoiDung.HoaDonDichVus ??= new List<HoaDonDichVu>();
            nguoiDung.HoaDonDoanhNghieps ??= new List<HoaDonDoanhNghiep>();
            nguoiDung.HoaDonSims ??= new List<HoaDonSim>();
            nguoiDung.Sims ??= new List<Sim>();

            TempData["ThanhCong"] = "Thông tin hồ sơ được tải thành công.";
            ViewBag.Section = section; // Xác định phần nội dung hiển thị
            return View(nguoiDung);
        }
        [HttpGet]
        public IActionResult NapTien(int id)
        {
            // Lấy thông tin SIM từ cơ sở dữ liệu
            var sim = _context.Sims
                .FirstOrDefault(s => s.Idsim == id);

            if (sim == null)
            {
                TempData["ErrorMessage"] = "SIM không tồn tại.";
                return RedirectToAction("HoSoNguoiDung", "NguoiDung", new { section = "quanlysim" });
            }

            // Truyền thông tin SIM sang view
            return View(sim);
        }
        public IActionResult ChiTietSim(int id)
        {
            var sim = _context.Sims
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .Include(s => s.SimGoiDangKies)
                    .ThenInclude(sg => sg.IdgoiDangKyNavigation)
                .FirstOrDefault(s => s.Idsim == id);

            if (sim == null)
            {
                return NotFound();
            }

            return View(sim);
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatHoSo(IFormFile AnhDaiDien, NguoiDung nguoiDungMoi)
        {
            GetData();
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");
            if (string.IsNullOrEmpty(tenDangNhap))
            {
                TempData["Loi"] = "Bạn cần đăng nhập để cập nhật thông tin.";
                return RedirectToAction("DangNhap");
            }

            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);
            if (nguoiDung == null)
            {
                TempData["Loi"] = "Không tìm thấy tài khoản người dùng.";
                return RedirectToAction("DangNhap");
            }

            nguoiDung.HoTen = nguoiDungMoi.HoTen;
            nguoiDung.Email = nguoiDungMoi.Email;
            nguoiDung.SoDienThoai = nguoiDungMoi.SoDienThoai;
            nguoiDung.DiaChi = nguoiDungMoi.DiaChi;
            nguoiDung.Cccd = nguoiDungMoi.Cccd;
            nguoiDung.AnhDaiDien = Upload(AnhDaiDien);

            _context.Update(nguoiDung);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = "Thông tin hồ sơ đã được cập nhật thành công.";
            return RedirectToAction("HoSoNguoiDung");
        }
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\user\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }
        public async Task<IActionResult> ChiTietHoaDonDichVu(int id)
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


        // Chi tiết hóa đơn doanh nghiệp
        public async Task<IActionResult> ChiTietHoaDonDoanhNghiep(int id)
        {
            GetData();
            var hoaDon = await _context.HoaDonDoanhNghieps
                .Include(x => x.CthoaDonDoanhNghieps)
                    .ThenInclude(x => x.IdgoiDichVuNavigation)
                .Include(x => x.IdnguoiDungNavigation) // Thêm thông tin người dùng
                .FirstOrDefaultAsync(x => x.IdhoaDonDn == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }


        public async Task<IActionResult> ChiTietHoaDonSim(int id)
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

        public async Task<IActionResult> ChiTietHoaDonDichVuKhac(int id)
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
    }
}
