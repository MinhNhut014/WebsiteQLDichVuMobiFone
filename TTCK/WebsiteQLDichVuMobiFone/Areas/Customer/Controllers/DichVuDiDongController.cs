using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;
using WebsiteQLDichVuMobiFone.Models.ViewModels;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DichVuDiDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuDiDongController(ApplicationDbContext context)
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
        public async Task<IActionResult> Index(string searchTerm, List<int> selectedCategories, int page = 1)
        {
            GetData();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            const int pageSize = 15; // Number of items per page

            // Lấy danh sách danh mục dịch vụ di động cùng với loại gói đăng ký
            var danhMucDichVu = _context.LoaiDichVuDiDongs
                                        .Select(dm => new LoaiDichVuDiDong
                                        {
                                            IdloaiDichVu = dm.IdloaiDichVu,
                                            TenLoaiDichVu = dm.TenLoaiDichVu,
                                            LoaiGoiDangKies = dm.LoaiGoiDangKies.ToList()
                                        }).ToList();

            // Lấy danh sách gói đăng ký
            var goiDangKy = _context.GoiDangKies.AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                goiDangKy = goiDangKy.Where(g => g.TenGoi.Contains(searchTerm));
            }

            // Lọc theo danh mục được chọn
            if (selectedCategories != null && selectedCategories.Any())
            {
                goiDangKy = goiDangKy.Where(g => selectedCategories.Contains(g.IdloaiGoi));
            }

            // Pagination logic
            var totalItems = await goiDangKy.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var paginatedGoiDangKy = await goiDangKy
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.GoiDangKy = paginatedGoiDangKy;
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(danhMucDichVu);
        }
        public async Task<IActionResult> ChiTietDichVu(int id)
        {
            GetData();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            var goiDangKy = await _context.GoiDangKies
                .FirstOrDefaultAsync(g => g.IdgoiDangKy == id);

            if (goiDangKy == null)
            {
                return NotFound();
            }

            var goiCuocTuongTu = await _context.GoiDangKies
                .Where(g => g.IdloaiGoi == goiDangKy.IdloaiGoi && g.IdgoiDangKy != id)
                .Take(3)
                .ToListAsync();

            ViewBag.GoiCuocTuongTu = goiCuocTuongTu;

            return View(goiDangKy);
        }
        [HttpGet]
        public IActionResult DangKyDichVu(int id)
        {
            GetData();

            // Kiểm tra người dùng đã đăng nhập hay chưa
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                // Nếu chưa đăng nhập, đặt thông báo vào TempData và chuyển hướng về trang trước đó
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để mua đăng ký.";
                var referrer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referrer))
                {
                    return Redirect(referrer);
                }
                return RedirectToAction("Index");
            }

            // Tìm kiếm gói đăng ký trong cơ sở dữ liệu
            var goiDangKy = _context.GoiDangKies.FirstOrDefault(g => g.IdgoiDangKy == id);

            // Kiểm tra gói đăng ký có tồn tại không
            if (goiDangKy == null)
            {
                TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                return RedirectToAction("Index");  // Điều hướng về trang chủ hoặc trang khác phù hợp
            }

            ViewBag.GoiDaChon = goiDangKy;
            ViewBag.TongTien = goiDangKy.GiaGoi;
            return View();
        }
        private string GenerateUniqueInvoiceCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newCode;
            do
            {
                // Phát sinh mã hóa đơn theo định dạng: HD-YYYYMMDD-XXXXXX
                var random = new Random();
                var randomString = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                newCode = $"HDDD-{DateTime.Now:yyyyMMdd}-{randomString}";
            }
            while (_context.HoaDonDichVus.Any(h => h.MaHoaDonDichVu == newCode)); // Kiểm tra trùng lặp

            return newCode;
        }
        // Xử lý hoàn tất đăng ký dịch vụ
        [HttpPost]
        public async Task<IActionResult> HoanTatDangKyDichVu(HoaDonDichVu hoaDon, int idGoiDangKy, [FromForm] string SoDienThoai)
        {
            GetData();

            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            if (string.IsNullOrEmpty(SoDienThoai))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập số điện thoại.";
                return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
            }

            try
            {
                var sim = await _context.Sims.FirstOrDefaultAsync(s => s.SoThueBao == SoDienThoai);
                if (sim == null || sim.IdtrangThaiSim != 3)
                {
                    TempData["ErrorMessage"] = "SIM không tồn tại hoặc chưa kích hoạt.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                var goiDangKy = await _context.GoiDangKies.FindAsync(idGoiDangKy);
                if (goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                bool daDangKy = await _context.SimGoiDangKies.AnyAsync(sg =>
                    sg.Idsim == sim.Idsim && sg.IdgoiDangKy == idGoiDangKy);

                if (daDangKy)
                {
                    TempData["ErrorMessage"] = "Số thuê bao này đã đăng ký gói dịch vụ này rồi.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                // ✅ Kiểm tra số dư SIM
                decimal soDuHienTai = sim.SoDu ?? 0;
                decimal giaGoi = goiDangKy.GiaGoi ?? 0;

                if (soDuHienTai < giaGoi)
                {
                    TempData["ErrorMessage"] = "Số dư không đủ để thanh toán gói dịch vụ.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                // ✅ Tiếp tục xử lý đăng ký nếu đủ tiền
                hoaDon.MaHoaDonDichVu = GenerateUniqueInvoiceCode();
                hoaDon.SoDienThoai = SoDienThoai;
                hoaDon.IdnguoiDung = int.Parse(nguoiDungId);
                hoaDon.NgayDatHang = DateTime.Now;
                hoaDon.PhuongThucThanhToan = "Thanh toán trực tiếp";
                hoaDon.IdtrangThaiThanhToan = 2;
                hoaDon.IdtrangThai = 3;
                hoaDon.TongTien = (int)giaGoi;

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.HoaDonDichVus.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    var chiTietHoaDon = new CthoaDonDichVu
                    {
                        IdhoaDonDv = hoaDon.IdhoaDonDv,
                        IdgoiDangKy = idGoiDangKy,
                        DonGia = (int)giaGoi,
                        SoLuong = 1,
                        ThanhTien = (int)giaGoi
                    };
                    _context.CthoaDonDichVus.Add(chiTietHoaDon);
                    await _context.SaveChangesAsync();

                    var simGoiDangKy = new SimGoiDangKy
                    {
                        Idsim = sim.Idsim,
                        IdgoiDangKy = idGoiDangKy,
                        NgayDangKy = DateTime.Now
                    };
                    _context.SimGoiDangKies.Add(simGoiDangKy);

                    // ✅ Trừ tiền trong tài khoản SIM
                    sim.SoDu = soDuHienTai - giaGoi;
                    _context.Sims.Update(sim);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Đăng ký dịch vụ thành công.";
                    return RedirectToAction("ThongBaoHoanTat", new { idHoaDon = hoaDon.IdhoaDonDv });
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Lỗi cơ sở dữ liệu: " + dbEx.InnerException?.Message;
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Lỗi không xác định: " + ex.Message;
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
                return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
            }
        }

        public IActionResult ThongBaoHoanTat(int idHoaDon)
        {
            GetData();
            var hoaDonDichVu = _context.HoaDonDichVus
                                    .FirstOrDefault(h => h.IdhoaDonDv == idHoaDon);

            if (hoaDonDichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng dịch vụ.";
                return RedirectToAction("Index", "Home");
            }

            return View(hoaDonDichVu);
        }

    }
}
