﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Areas.Customer.ViewModels;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;
using WebsiteQLDichVuMobiFone.Services.VNPay;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVnPayService _vnPayService;
        public SimController(ApplicationDbContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
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
        public IActionResult Index(string filters, string search, int page = 1)
        {
            GetData();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            const int pageSize = 15; // Number of items per page

            var sims = _context.Sims
                            .Include(s => s.IdloaiSoNavigation)
                            .Where(s => s.IdtrangThaiSim == 1) // Chỉ hiển thị SIM ở trạng thái 1
                            .OrderByDescending(s => s.Idsim)    // Hiển thị các SIM mới nhất lên đầu
                            .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                sims = sims.Where(s => s.SoThueBao.Contains(search));
            }

            var selectedFilters = string.IsNullOrEmpty(filters) ? new List<string>() : filters.Split(',').ToList();

            if (selectedFilters.Any())
            {
                sims = sims.Where(s =>
                    selectedFilters.Contains(s.SoThueBao.Substring(0, 3)) ||
                    selectedFilters.Contains(s.IdloaiSoNavigation.TenLoaiSo)
                );
            }

            // Pagination logic
            var totalItems = sims.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var paginatedSims = sims
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.DauSos = _context.Sims.Select(s => s.SoThueBao.Substring(0, 3)).Distinct().ToList();
            ViewBag.LoaiThueBaos = _context.LoaiSos.Select(l => l.TenLoaiSo).Distinct().ToList();
            ViewBag.SelectedFilters = selectedFilters;
            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedSims);
        }

        public IActionResult ChonMua(int id, int? maGoi)
        {
            GetData();

            // Kiểm tra người dùng đã đăng nhập hay chưa
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                // Nếu chưa đăng nhập, đặt thông báo vào ViewBag và chuyển hướng về trang Index
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để mua SIM.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin SIM cần mua
            var sim = _context.Sims.Include(s => s.IdloaiSoNavigation).FirstOrDefault(s => s.Idsim == id);
            if (sim == null)
            {
                TempData["Error"] = "SIM không tồn tại!";
                return RedirectToAction("Index");
            }

            // Lấy danh sách gói cước mặc định
            var goiCuoc = _context.GoiDangKies
                    .Where(g => g.GiaGoi == 90000 && g.ThoiHan == "30")
                    .Take(3)
                    .ToList();


            if (goiCuoc == null || goiCuoc.Count == 0)
            {
                TempData["Error"] = "Không có gói cước khả dụng!";
                return RedirectToAction("Index");
            }

            ViewBag.GoiDangKy = goiCuoc;
            ViewBag.Sim = sim;
            ViewBag.MaGoiDaChon = maGoi?.ToString();
            // Xóa dữ liệu cũ trong TempData trước khi thêm mới
            TempData.Remove("Cart");

            // Gói cước mặc định hoặc gói được chọn
            var goiChon = maGoi.HasValue
                ? goiCuoc.FirstOrDefault(g => g.IdgoiDangKy == maGoi.Value)
                : goiCuoc.FirstOrDefault();

            if (goiChon != null)
            {
                // Tạo đối tượng Cart mới
                var cartItem = new CartViewModel
                {
                    Idsim = sim.Idsim,
                    SoThueBao = sim.SoThueBao,
                    LoaiThueBao = sim.IdloaiSoNavigation?.TenLoaiSo ?? "Không xác định",
                    IdgoiDangKy = goiChon.IdgoiDangKy,
                    TenGoi = goiChon.TenGoi,
                    GiaGoi = goiChon.GiaGoi
                };

                // Chuyển đối tượng thành danh sách để đảm bảo cấu trúc dữ liệu
                var cart = new List<CartViewModel> { cartItem };
                ViewBag.MaGoiDaChon = goiChon?.IdgoiDangKy.ToString();
                // Lưu dữ liệu mới vào TempData
                TempData["Cart"] = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
                TempData.Keep("Cart"); // Giữ TempData sống sót sau redirect
            }

            return View();
        }


        /// chức năng đăng ksy đơn hàng sim
        // Action chuyển qua trang đăng ký SIM
        [HttpGet]
        public async Task<IActionResult> DangKy(int idSim, int idGoiDangKy)
        {
            GetData();
            // Lấy thông tin SIM
            var sim = await _context.Sims
                .Include(s => s.IdloaiSoNavigation)
                .FirstOrDefaultAsync(s => s.Idsim == idSim);

            if (sim == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin SIM!";
                return RedirectToAction("ChonMua");
            }

            // Lấy thông tin gói đăng ký
            var goiDangKy = await _context.GoiDangKies
                .FirstOrDefaultAsync(g => g.IdgoiDangKy == idGoiDangKy);

            if (goiDangKy == null)
            {
                TempData["Error"] = "Gói đăng ký không hợp lệ!";
                return RedirectToAction("ChonMua");
            }

            // Lấy danh sách phương thức vận chuyển
            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens.ToListAsync();

            if (phuongThucVanChuyen == null || !phuongThucVanChuyen.Any())
            {
                TempData["Error"] = "Không có phương thức vận chuyển nào!";
                return RedirectToAction("ChonMua");
            }

            // Truyền dữ liệu qua ViewBag
            ViewBag.Sim = sim;
            ViewBag.GoiDaChon = goiDangKy;
            ViewBag.PhuongThucVanChuyen = phuongThucVanChuyen;

            // Tính tổng tiền ban đầu (xử lý null)
            decimal phiHoaMang = sim.PhiHoaMang ?? 0;
            decimal giaGoi = goiDangKy.GiaGoi ?? 0;
            decimal tongTien = phiHoaMang + giaGoi;

            // Truyền tổng tiền chưa bao gồm phí vận chuyển
            ViewBag.TongTien = tongTien;

            // Truyền phí vận chuyển (chưa tính)
            ViewBag.PhiVanChuyen = 0; // Mặc định chưa có phí vận chuyển, sẽ được tính sau khi người dùng chọn phương thức vận chuyển

            return View("DangKy");
        }

        // hoàn tất đăng ký sim
        // hoàn tất đăng ký sim
        [HttpPost]
        public IActionResult HoanTatDangKySim(HoaDonSim hoaDonModel, int idSim, int idGoiDangKy)
        {
            GetData();

            // Kiểm tra người dùng đã đăng nhập hay chưa
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                // Nếu chưa đăng nhập, thông báo và yêu cầu đăng nhập
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thực hiện mua SIM.";
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            // Nếu người dùng đã đăng nhập, gán idNguoiDung từ session
            hoaDonModel.IdnguoiDung = int.Parse(nguoiDungId);

            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.";
                return RedirectToAction("DangKy", "Sim", new { idSim, idGoiDangKy });
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Kiểm tra tính hợp lệ của Phương thức Vận chuyển
                var phuongThucVanChuyen = _context.PhuongThucVanChuyens.FirstOrDefault(p => p.IdphuongThucVc == hoaDonModel.IdphuongThucVc);
                if (phuongThucVanChuyen == null)
                {
                    TempData["ErrorMessage"] = "Phương thức vận chuyển không hợp lệ.";
                    return RedirectToAction("DangKy", "Sim", new { idSim, idGoiDangKy }); // Quay lại trang đăng ký nếu không có phương thức vận chuyển hợp lệ
                }

                var sim = _context.Sims
                                  .Include(s => s.IdtrangThaiSimNavigation)  // Bao gồm IdtrangThaiSimNavigation
                                  .Include(s => s.GoiDangKyDiKemNavigation)  // Bao gồm GoiDangKyDiKemNavigation
                                  .FirstOrDefault(s => s.Idsim == idSim);
                var goiDangKy = _context.GoiDangKies.AsNoTracking().FirstOrDefault(g => g.IdgoiDangKy == idGoiDangKy);

                if (sim == null || goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Thông tin SIM hoặc gói đăng ký không hợp lệ.";
                    return RedirectToAction("DangKy", "Sim", new { idSim, idGoiDangKy });
                }

                decimal phiHoaMang = sim.PhiHoaMang ?? 0;
                decimal giaGoi = goiDangKy.GiaGoi ?? 0;
                decimal phiVanChuyen = phuongThucVanChuyen.GiaVanChuyen ?? 0;
                int tongTien = (int)Math.Round(phiHoaMang + giaGoi + phiVanChuyen);

                // Store customer information in session
                HttpContext.Session.SetString("TenKhachHang", hoaDonModel.TenKhachHang);
                HttpContext.Session.SetString("SoDienThoai", hoaDonModel.SoDienThoai);
                HttpContext.Session.SetString("Email", hoaDonModel.Email);
                HttpContext.Session.SetString("DiaDiemNhan", hoaDonModel.DiaDiemNhan);
                HttpContext.Session.SetString("PhuongThucThanhToan", hoaDonModel.PhuongThucThanhToan);

                // Thêm Hóa Đơn SIM
                var hoaDon = new HoaDonSim
                {
                    IdnguoiDung = hoaDonModel.IdnguoiDung,
                    NgayDatHang = DateTime.Now,
                    TongTien = tongTien,
                    IdtrangThai = 1, // Trạng thái "Chờ xử lý"
                    TenKhachHang = hoaDonModel.TenKhachHang,
                    SoDienThoai = hoaDonModel.SoDienThoai,
                    Email = hoaDonModel.Email,
                    DiaDiemNhan = hoaDonModel.DiaDiemNhan,
                    PhuongThucThanhToan = hoaDonModel.PhuongThucThanhToan,
                    IdphuongThucVc = hoaDonModel.IdphuongThucVc // Phương thức vận chuyển hợp lệ
                };

                _context.HoaDonSims.Add(hoaDon);
                _context.SaveChanges();

                // Sau khi đã lưu HoaDonSim, lấy IDHoaDonSim
                var idHoaDonSim = hoaDon.IdhoaDonSim;
                // Thêm Chi Tiết Hóa Đơn SIM
                var chiTietHoaDon = new CthoaDonSim
                {
                    IdhoaDonSim = idHoaDonSim,  // Đảm bảo rằng IDHoaDonSim đã tồn tại trong HoaDonSim
                    Idsim = idSim,
                    IdgoiDangKy = idGoiDangKy,
                    DonGia = hoaDonModel.TongTien,
                    SoLuong = 1,
                    ThanhTien = tongTien
                };

                _context.CthoaDonSims.Add(chiTietHoaDon);

                // Cập nhật trạng thái SIM
                if (sim != null)
                {
                    // Kiểm tra nếu IdtrangThaiSimNavigation không phải là null
                    if (sim.IdtrangThaiSimNavigation != null)
                    {
                        sim.IdtrangThaiSim = 2; // "Đang hoạt động"
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy thông tin trạng thái SIM.");
                    }

                    // Kiểm tra nếu GoiDangKyDiKemNavigation là null, gán gói đăng ký mới nếu có
                    if (sim.GoiDangKyDiKemNavigation == null)
                    {
                        if (goiDangKy != null)
                        {
                            // Thay vì tạo mới, gán đối tượng GoidangKy đã lấy từ DB vào GoiDangKyDiKemNavigation
                            sim.GoiDangKyDiKemNavigation = goiDangKy;
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy gói đăng ký đi kèm.");
                        }
                    }
                    else
                    {
                        // Trường hợp GoiDangKyDiKemNavigation đã có, kiểm tra và cập nhật lại
                        if (goiDangKy != null)
                        {
                            sim.GoiDangKyDiKemNavigation = goiDangKy;
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy gói đăng ký đi kèm.");
                        }
                    }

                    // Cập nhật SIM và lưu thay đổi
                    _context.Sims.Update(sim);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy thông tin SIM.");
                }

                _context.SaveChanges();
                transaction.Commit();

                TempData["SuccessMessage"] = "Đăng ký SIM thành công.";
                return RedirectToAction("ThongBaoHoanTat", "Sim", new { idHoaDon = hoaDon.IdhoaDonSim });
            }
            catch (DbUpdateException dbEx)
            {
                transaction.Rollback();
                TempData["ErrorMessage"] = "Có lỗi khi cập nhật cơ sở dữ liệu: " + dbEx.Message;
                if (dbEx.InnerException != null)
                {
                    TempData["ErrorMessage"] += " | Inner Exception: " + dbEx.InnerException.Message;
                }
                return RedirectToAction("DangKy", "Sim", new { idSim, idGoiDangKy });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đăng ký SIM: " + ex.Message;
                if (ex.InnerException != null)
                {
                    TempData["ErrorMessage"] += " | Inner Exception: " + ex.InnerException.Message;
                }
                return RedirectToAction("DangKy", "Sim", new { idSim, idGoiDangKy });
            }
        }


        public IActionResult ThongBaoHoanTat(int idHoaDon)
        {
            GetData();
            var hoaDonSim = _context.HoaDonSims
                                    .Include(h => h.IdphuongThucVcNavigation)
                                    .FirstOrDefault(h => h.IdhoaDonSim == idHoaDon);

            if (hoaDonSim == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Index", "Home");
            }

            return View(hoaDonSim);
        }


        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            GetData();
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || !response.Success)
            {
                TempData["ErrorMessage"] = "Thanh toán không thành công.";
                return RedirectToAction("Index", "Sim");
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Retrieve customer information from session
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

                var sim = _context.Sims
                                  .Include(s => s.IdtrangThaiSimNavigation)
                                  .Include(s => s.GoiDangKyDiKemNavigation)
                                  .FirstOrDefault(s => s.Idsim == idSim);

                if (sim == null)
                {
                    TempData["ErrorMessage"] = "Thông tin SIM không hợp lệ.";
                    return RedirectToAction("Index", "Sim");
                }

                var goiDangKy = _context.GoiDangKies.AsNoTracking().FirstOrDefault(g => g.IdgoiDangKy == idGoiDangKy);

                if (goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Thông tin gói đăng ký không hợp lệ.";
                    return RedirectToAction("Index", "Sim");
                }

                var phuongThucVanChuyen = _context.PhuongThucVanChuyens.FirstOrDefault(p => p.IdphuongThucVc == idPhuongThucVc);
                if (phuongThucVanChuyen == null)
                {
                    TempData["ErrorMessage"] = "Phương thức vận chuyển không hợp lệ.";
                    return RedirectToAction("Index", "Sim");
                }

                decimal phiHoaMang = sim.PhiHoaMang ?? 0;
                decimal giaGoi = goiDangKy.GiaGoi ?? 0;
                decimal phiVanChuyen = phuongThucVanChuyen.GiaVanChuyen ?? 0;
                int tongTien = (int)Math.Round(phiHoaMang + giaGoi + phiVanChuyen);

                // Thêm Hóa Đơn SIM
                var hoaDon = new HoaDonSim
                {
                    IdnguoiDung = int.Parse(nguoiDungId),
                    NgayDatHang = DateTime.Now,
                    TongTien = tongTien,
                    IdtrangThai = 1, // Trạng thái "Chờ xử lý"
                    TenKhachHang = tenKhachHang,
                    SoDienThoai = soDienThoai,
                    Email = email,
                    DiaDiemNhan = diaDiemNhan,
                    PhuongThucThanhToan = phuongThucThanhToan,
                    IdphuongThucVc = idPhuongThucVc.Value
                };

                _context.HoaDonSims.Add(hoaDon);
                _context.SaveChanges();

                // Sau khi đã lưu HoaDonSim, lấy IDHoaDonSim
                var idHoaDonSim = hoaDon.IdhoaDonSim;

                // Thêm Chi Tiết Hóa Đơn SIM
                var chiTietHoaDon = new CthoaDonSim
                {
                    IdhoaDonSim = idHoaDonSim,
                    Idsim = idSim.Value,
                    IdgoiDangKy = idGoiDangKy.Value,
                    DonGia = tongTien,
                    SoLuong = 1,
                    ThanhTien = tongTien
                };

                _context.CthoaDonSims.Add(chiTietHoaDon);

                // Cập nhật trạng thái SIM
                if (sim != null)
                {
                    // Kiểm tra nếu IdtrangThaiSimNavigation không phải là null
                    if (sim.IdtrangThaiSimNavigation != null)
                    {
                        sim.IdtrangThaiSim = 2; // "Đang hoạt động"
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy thông tin trạng thái SIM.");
                    }

                    // Kiểm tra nếu GoiDangKyDiKemNavigation là null, gán gói đăng ký mới nếu có
                    if (sim.GoiDangKyDiKemNavigation == null)
                    {
                        if (goiDangKy != null)
                        {
                            // Thay vì tạo mới, gán đối tượng GoidangKy đã lấy từ DB vào GoiDangKyDiKemNavigation
                            sim.GoiDangKyDiKemNavigation = goiDangKy;
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy gói đăng ký đi kèm.");
                        }
                    }
                    else
                    {
                        // Trường hợp GoiDangKyDiKemNavigation đã có, kiểm tra và cập nhật lại
                        if (goiDangKy != null)
                        {
                            sim.GoiDangKyDiKemNavigation = goiDangKy;
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy gói đăng ký đi kèm.");
                        }
                    }

                    // Cập nhật SIM và lưu thay đổi
                    _context.Sims.Update(sim);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy thông tin SIM.");
                }

                _context.SaveChanges();
                transaction.Commit();

                TempData["SuccessMessage"] = "Thanh toán thành công.";
                return RedirectToAction("ThongBaoHoanTat", "Sim", new { idHoaDon = idHoaDonSim });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", "Sim");
            }
        }
        public IActionResult ThongBaoThatBai()
        {
            return View();
        }
    }
}
