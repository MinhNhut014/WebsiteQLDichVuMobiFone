using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;
using WebsiteQLDichVuMobiFone.Models.VNPay;
using WebsiteQLDichVuMobiFone.Services.VNPay;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        /// <summary>
        /// Lấy thông tin người dùng từ Session và truyền vào ViewBag.
        /// </summary>
        public void GetData()
        {
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");

            if (!string.IsNullOrEmpty(tenDangNhap))
            {
                ViewBag.khachHang = _context.NguoiDungs.FirstOrDefault(k => k.TenDangNhap == tenDangNhap);
            }

            ViewBag.UserName = tenDangNhap;
            ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
        }

        /// <summary>
        /// Tạo URL thanh toán VNPay dựa trên thông tin dịch vụ.
        /// </summary>
        /// <param name="model">Thông tin thanh toán</param>
        /// <returns>Redirect đến URL thanh toán VNPay</returns>
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            try
            {
                // Lưu thông tin vào Session để sử dụng trong callback
                HttpContext.Session.SetString("LoaiDichVu", model.LoaiDichVu);
                HttpContext.Session.SetString("TenKhachHang", model.TenKhachHang);
                HttpContext.Session.SetString("SoDienThoai", model.SoDienThoai);
                HttpContext.Session.SetString("Email", model.Email);
                HttpContext.Session.SetInt32("IdGoiDangKy", model.IdGoiDangKy);
                HttpContext.Session.SetString("PhuongThucThanhToan", model.PhuongThucThanhToan);

                if (model.LoaiDichVu == "sim" || model.LoaiDichVu == "naptien")
{
    HttpContext.Session.SetInt32("IdSim", model.IdSim);
}

if (model.LoaiDichVu == "sim")
{
    HttpContext.Session.SetInt32("IdPhuongThucVc", model.IdPhuongThucVc);
    HttpContext.Session.SetString("DiaDiemNhan", model.DiaDiemNhan);
}

                // Tạo URL thanh toán
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

                return Redirect(url);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi tạo URL thanh toán: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Xử lý callback từ VNPay sau khi thanh toán.
        /// </summary>
        /// <returns>Redirect đến trang thông báo kết quả</returns>
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            GetData();
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);

                // Lấy loại dịch vụ từ Session
                var loaiDichVu = HttpContext.Session.GetString("LoaiDichVu")?.ToLower();

                if (response.Success && response.VnPayResponseCode == "00" && loaiDichVu == "didong")
                {
                    return await HandlePaymentForMobileService(response);
                }
                else if (response.Success && response.VnPayResponseCode == "00" && loaiDichVu == "sim")
                {
                    return await HandlePaymentForSim(response);
                }
                else if (response.Success && response.VnPayResponseCode == "00" && loaiDichVu == "dichvukhac")
                {
                    return await HandlePaymentForOtherService(response);
                }
                else if (response.Success && response.VnPayResponseCode == "00" && loaiDichVu == "naptien")
                {
                    return await HandlePaymentForNapTien(response);
                }
                else
                {
                    TempData["ErrorMessage"] = "Thanh toán không thành công hoặc sai loại dịch vụ.";
                    return RedirectToAction("Index", "Sim");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi xử lý callback: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Xử lý thanh toán cho dịch vụ di động.
        /// </summary>
        /// <param name="response">Kết quả thanh toán từ VNPay</param>
        /// <returns>Redirect đến trang thông báo kết quả</returns>
        private async Task<IActionResult> HandlePaymentForMobileService(PaymentResponseModel response)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var idGoiDangKy = HttpContext.Session.GetInt32("IdGoiDangKy");
                var tenKhachHang = HttpContext.Session.GetString("TenKhachHang");
                var soDienThoai = HttpContext.Session.GetString("SoDienThoai");
                var email = HttpContext.Session.GetString("Email");
                var nguoiDungId = HttpContext.Session.GetString("UserId");
                var phuongThucThanhToan = HttpContext.Session.GetString("PhuongThucThanhToan");

                if (string.IsNullOrEmpty(nguoiDungId) || idGoiDangKy == null || string.IsNullOrEmpty(soDienThoai))
                {
                    TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                    return RedirectToAction("Index", "DichVuDiDong");
                }

                var sim = await _context.Sims.FirstOrDefaultAsync(s => s.SoThueBao == soDienThoai);
                var goiDangKy = await _context.GoiDangKies.FindAsync(idGoiDangKy);

                if (sim == null || sim.IdtrangThaiSim != 3 || goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "SIM chưa kích hoạt hoặc gói dịch vụ không tồn tại.";
                    return RedirectToAction("DangKyDichVu", "DichVuDiDong", new { id = idGoiDangKy });
                }

                // Kiểm tra trùng đăng ký
                bool daDangKy = await _context.SimGoiDangKies.AnyAsync(sg =>
                    sg.Idsim == sim.Idsim && sg.IdgoiDangKy == idGoiDangKy);

                if (daDangKy)
                {
                    TempData["ErrorMessage"] = "Số thuê bao đã đăng ký gói này.";
                    return RedirectToAction("DangKyDichVu", "DichVuDiDong", new { id = idGoiDangKy });
                }

                var hoaDon = new HoaDonDichVu
                {
                    SoDienThoai = soDienThoai,
                    IdnguoiDung = int.Parse(nguoiDungId),
                    TenKhachHang = tenKhachHang,
                    Email = email,
                    NgayDatHang = DateTime.Now,
                    IdtrangThaiThanhToan = 2,
                    IdtrangThai = 3, // Chờ xử lý
                    TongTien = goiDangKy.GiaGoi, 
                    PhuongThucThanhToan = phuongThucThanhToan,
                    MaHoaDonDichVu = response.OrderId
                };

                _context.HoaDonDichVus.Add(hoaDon);
                await _context.SaveChangesAsync();

                var chiTiet = new CthoaDonDichVu
                {
                    IdhoaDonDv = hoaDon.IdhoaDonDv,
                    IdgoiDangKy = idGoiDangKy.Value,
                    DonGia = goiDangKy.GiaGoi,
                    SoLuong = 1,
                    ThanhTien = goiDangKy.GiaGoi
                };

                _context.CthoaDonDichVus.Add(chiTiet);

                var simGoi = new SimGoiDangKy
                {
                    Idsim = sim.Idsim,
                    IdgoiDangKy = goiDangKy.IdgoiDangKy,
                    NgayDangKy = DateTime.Now
                };

                _context.SimGoiDangKies.Add(simGoi);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đăng ký dịch vụ thành công.";
                return RedirectToAction("ThongBaoHoanTat", "DichVuDiDong", new { idHoaDon = hoaDon.IdhoaDonDv });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + innerMessage;
                return RedirectToAction("Index", "DichVuDiDong");
            }
        }

        /// <summary>
        /// Xử lý thanh toán cho SIM.
        /// </summary>
        /// <param name="response">Kết quả thanh toán từ VNPay</param>
        /// <returns>Redirect đến trang thông báo kết quả</returns>
        private async Task<IActionResult> HandlePaymentForSim(PaymentResponseModel response)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var idSim = HttpContext.Session.GetInt32("IdSim");
                var idGoiDangKy = HttpContext.Session.GetInt32("IdGoiDangKy");
                var idPhuongThucVc = HttpContext.Session.GetInt32("IdPhuongThucVc");
                var tenKhachHang = HttpContext.Session.GetString("TenKhachHang");
                var soDienThoai = HttpContext.Session.GetString("SoDienThoai");
                var email = HttpContext.Session.GetString("Email");
                var diaDiemNhan = HttpContext.Session.GetString("DiaDiemNhan");
                var phuongThucThanhToan = HttpContext.Session.GetString("PhuongThucThanhToan");
                var nguoiDungId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(nguoiDungId) || idSim == null || idGoiDangKy == null || idPhuongThucVc == null)
                {
                    TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                    return RedirectToAction("Index", "Sim");
                }

                var sim = await _context.Sims.FindAsync(idSim);
                var goiDangKy = await _context.GoiDangKies.FindAsync(idGoiDangKy);
                var phuongThucVanChuyen = await _context.PhuongThucVanChuyens.FindAsync(idPhuongThucVc);

                if (sim == null || goiDangKy == null || phuongThucVanChuyen == null)
                {
                    TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                    return RedirectToAction("Index", "Sim");
                }

                decimal phiHoaMang = sim.PhiHoaMang ?? 0;
                decimal giaGoi = goiDangKy.GiaGoi ?? 0;
                decimal phiVanChuyen = phuongThucVanChuyen.GiaVanChuyen ?? 0;
                int tongTien = (int)Math.Round(phiHoaMang + giaGoi + phiVanChuyen);

                var hoaDon = new HoaDonSim
                {
                    IdnguoiDung = int.Parse(nguoiDungId),
                    NgayDatHang = DateTime.Now,
                    TongTien = tongTien,
                    IdtrangThai = 1,
                    IdtrangThaiThanhToan = 2, // Đã thanh toán
                    TenKhachHang = tenKhachHang,
                    SoDienThoai = soDienThoai,
                    Email = email,
                    DiaDiemNhan = diaDiemNhan,
                    PhuongThucThanhToan = phuongThucThanhToan,
                    IdphuongThucVc = idPhuongThucVc.Value,
                    MaHoaDonSim = response.OrderId
                };

                _context.HoaDonSims.Add(hoaDon);
                await _context.SaveChangesAsync();

                var chiTietHoaDon = new CthoaDonSim
                {
                    IdhoaDonSim = hoaDon.IdhoaDonSim,
                    Idsim = idSim.Value,
                    IdgoiDangKy = idGoiDangKy.Value,
                    DonGia = tongTien,
                    SoLuong = 1,
                    ThanhTien = tongTien
                };

                _context.CthoaDonSims.Add(chiTietHoaDon);

                sim.IdtrangThaiSim = 2; // Cập nhật trạng thái SIM
                sim.IdnguoiDung = hoaDon.IdnguoiDung;
                _context.Sims.Update(sim);

                var simGoi = new SimGoiDangKy
                {
                    Idsim = sim.Idsim,
                    IdgoiDangKy = goiDangKy.IdgoiDangKy,
                    NgayDangKy = DateTime.Now
                };
                _context.SimGoiDangKies.Add(simGoi);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Thanh toán thành công.";
                return RedirectToAction("ThongBaoHoanTat", "Sim", new { idHoaDon = hoaDon.IdhoaDonSim });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", "Sim");
            }
        }
        private async Task<IActionResult> HandlePaymentForOtherService(PaymentResponseModel response)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var idGoiDangKy = HttpContext.Session.GetInt32("IdGoiDangKy");
                var tenKhachHang = HttpContext.Session.GetString("TenKhachHang");
                var soDienThoai = HttpContext.Session.GetString("SoDienThoai");
                var email = HttpContext.Session.GetString("Email");
                var phuongThucThanhToan = HttpContext.Session.GetString("PhuongThucThanhToan");
                var nguoiDungId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(nguoiDungId) || idGoiDangKy == null || string.IsNullOrEmpty(soDienThoai))
                {
                    TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                    return RedirectToAction("Index", "DichVuKhac");
                }

                var sim = await _context.Sims.FirstOrDefaultAsync(s => s.SoThueBao == soDienThoai);
                var goiDangKy = await _context.GoiDangKyDichVuKhacs.FindAsync(idGoiDangKy);

                if (sim == null || sim.IdtrangThaiSim != 3 || goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "SIM chưa kích hoạt hoặc gói dịch vụ không tồn tại.";
                    return RedirectToAction("DangKyDichVu", "DichVuKhac", new { id = idGoiDangKy });
                }

                // Kiểm tra trùng đăng ký
                bool daDangKy = await _context.SimGoiDangKyDichVuKhacs.AnyAsync(sg =>
                    sg.Idsim == sim.Idsim && sg.IdgoiDangKy == idGoiDangKy);

                if (daDangKy)
                {
                    TempData["ErrorMessage"] = "Số thuê bao đã đăng ký gói này.";
                    return RedirectToAction("DangKyDichVu", "DichVuKhac", new { id = idGoiDangKy });
                }

                var hoaDon = new HoaDonDichVu
                {
                    SoDienThoai = soDienThoai,
                    IdnguoiDung = int.Parse(nguoiDungId),
                    TenKhachHang = tenKhachHang,
                    Email = email,
                    NgayDatHang = DateTime.Now,
                    IdtrangThaiThanhToan = 2,
                    IdtrangThai = 3, // Chờ xử lý
                    TongTien = goiDangKy.GiaGoi,
                    PhuongThucThanhToan = phuongThucThanhToan,
                    MaHoaDonDichVu = response.OrderId
                };

                _context.HoaDonDichVus.Add(hoaDon);
                await _context.SaveChangesAsync();

                var chiTiet = new CthoaDonDichVu
                {
                    IdhoaDonDv = hoaDon.IdhoaDonDv,
                    IdgoiDangKyDvk = idGoiDangKy.Value,
                    DonGia = goiDangKy.GiaGoi,
                    SoLuong = 1,
                    ThanhTien = goiDangKy.GiaGoi
                };

                _context.CthoaDonDichVus.Add(chiTiet);

                var simGoi = new SimGoiDangKyDichVuKhac
                {
                    Idsim = sim.Idsim,
                    IdgoiDangKy = goiDangKy.IdgoiDangKy,
                    NgayDangKy = DateTime.Now
                };

                _context.SimGoiDangKyDichVuKhacs.Add(simGoi);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đăng ký dịch vụ thành công.";
                return RedirectToAction("ThongBaoHoanTat", "DichVuKhac", new { idHoaDon = hoaDon.IdhoaDonDv });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + innerMessage;
                return RedirectToAction("Index", "DichVuKhac");
            }
        }
        private async Task<IActionResult> HandlePaymentForNapTien(PaymentResponseModel response)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy thông tin từ Session
                var idSim = HttpContext.Session.GetInt32("IdSim");
                var tenKhachHang = HttpContext.Session.GetString("TenKhachHang");
                var phuongThucThanhToan = HttpContext.Session.GetString("PhuongThucThanhToan");

                // Kiểm tra thông tin cần thiết
                if (idSim == null || response.Amount <= 0)
                {
                    TempData["ErrorMessage"] = "Thiếu thông tin giao dịch hoặc số tiền không hợp lệ.";
                    return RedirectToAction("NapTien", "NguoiDung", new { id = idSim });
                }

                // Lấy thông tin SIM từ cơ sở dữ liệu
                var sim = await _context.Sims.FirstOrDefaultAsync(s => s.Idsim == idSim);
                if (sim == null)
                {
                    TempData["ErrorMessage"] = "SIM không tồn tại.";
                    return RedirectToAction("NapTien", "NguoiDung", new { id = idSim });
                }

                // Lưu giao dịch nạp tiền
                var giaoDich = new GiaoDichNapTien
                {
                    Idsim = sim.Idsim,
                    IdnguoiDung = sim.IdnguoiDung, // Lấy ID người dùng từ SIM
                    NgayNap = DateTime.Now,
                    SoTienNap = Convert.ToDecimal(response.Amount), // VNPay trả về số tiền nhân 100, cần chia lại
                    PhuongThucNap = phuongThucThanhToan,
                    MaGiaoDichNapTien = response.OrderId,
                    IdtrangThaiThanhToan = 1 // Đã thanh toán
                };

                _context.GiaoDichNapTiens.Add(giaoDich);

                // Cộng tiền vào số dư SIM
                sim.SoDu = (sim.SoDu ?? 0) + giaoDich.SoTienNap;
                _context.Sims.Update(sim);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                ViewBag.SimSuccessMessage = "Nạp tiền thành công.";
                return RedirectToAction("HoSoNguoiDung", "NguoiDung", new { section = "quanlysim" });

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi nạp tiền: " + innerMessage;
                return RedirectToAction("NapTien", "NguoiDung", new { id = HttpContext.Session.GetInt32("IdSim") });
            }
        }

    }
}
