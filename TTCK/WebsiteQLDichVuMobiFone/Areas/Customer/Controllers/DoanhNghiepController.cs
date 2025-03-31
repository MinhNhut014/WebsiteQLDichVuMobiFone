using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DoanhNghiepController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoanhNghiepController(ApplicationDbContext context)
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
        public async Task<IActionResult> Index(string searchTerm, List<int> selectedCategories)
        {
            GetData();
            // Lấy danh sách nhóm dịch vụ doanh nghiệp cùng với dịch vụ doanh nghiệp
            var danhMucDichVu = _context.NhomDichVuDoanhNghieps
                                        .Select(ndv => new NhomDichVuDoanhNghiep
                                        {
                                            IdnhomDichVu = ndv.IdnhomDichVu,
                                            TenNhom = ndv.TenNhom,
                                            DichVuDoanhNghieps = ndv.DichVuDoanhNghieps.ToList()
                                        }).ToList();

            // Lấy danh sách gói dịch vụ
            var goiDichVu = _context.GoiDichVus.AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                goiDichVu = goiDichVu.Where(g => g.TenGoiDv.Contains(searchTerm));
            }

            // Lọc theo danh mục được chọn
            if (selectedCategories != null && selectedCategories.Any())
            {
                goiDichVu = goiDichVu.Where(g => selectedCategories.Contains(g.IddichVuDn));
            }

            ViewBag.GoiDichVu = goiDichVu.ToList();
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SearchTerm = searchTerm;

            return View(danhMucDichVu);
        }
        public async Task<IActionResult> ChiTietDichVu(int id)
        {
            var goiDichVu = await _context.GoiDichVus
                                         .Include(g => g.IddichVuDnNavigation)
                                         .ThenInclude(dv => dv.GoiDichVus)
                                         .FirstOrDefaultAsync(g => g.IdgoiDichVu == id);

            if (goiDichVu == null)
            {
                return NotFound();
            }

            // Lấy danh sách dịch vụ liên quan cùng nhóm nhưng loại trừ chính nó
            var dichVuLienQuan = goiDichVu.IddichVuDnNavigation?.GoiDichVus
                                        .Where(g => g.IdgoiDichVu != id)
                                        .ToList();

            ViewBag.DichVuLienQuan = dichVuLienQuan;

            return View(goiDichVu);
        }
        [HttpGet]
        public IActionResult DangKyDichVu(int id)
        {
            GetData();
            // Kiểm tra ID gói dịch vụ được truyền vào
            Console.WriteLine("ID được truyền vào: " + id);

            // Tìm kiếm gói đăng ký trong cơ sở dữ liệu
            var goiDangKy = _context.GoiDichVus.FirstOrDefault(g => g.IdgoiDichVu == id);

            // Kiểm tra gói đăng ký có tồn tại không
            if (goiDangKy == null)
            {
                TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                return RedirectToAction("Index");  // Điều hướng về trang chủ hoặc trang khác phù hợp
            }

            ViewBag.GoiDaChon = goiDangKy;
            return View();
        }
        // Xử lý hoàn tất đăng ký dịch vụ doanh nghiệp
        [HttpPost]
        public async Task<IActionResult> HoanTatDangKyDichVu(HoaDonDoanhNghiep hoaDon, int idGoiDangKy, [FromForm] string TenCongTy, [FromForm] string SoDienThoai, [FromForm] string Email, [FromForm] string DiaChi)
        {
            GetData();

            // Kiểm tra đăng nhập
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
                var goiDangKy = await _context.GoiDichVus.FindAsync(idGoiDangKy);
                if (goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }
                hoaDon.TenCongTy = TenCongTy;
                hoaDon.SoDienThoaiCongTy = SoDienThoai;
                hoaDon.EmailCongTy = Email;
                hoaDon.DiaChiCongTy = DiaChi;
                hoaDon.IdnguoiDung = int.Parse(nguoiDungId);
                hoaDon.NgayDatHang = DateTime.Now;
                hoaDon.IdtrangThai = 1;
                hoaDon.TongTien = 0;

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    Console.WriteLine($"Thông tin hóa đơn: SĐT - {hoaDon.SoDienThoaiCongTy}, Email - {hoaDon.EmailCongTy}, Địa chỉ - {hoaDon.DiaChiCongTy}");

                    _context.HoaDonDoanhNghieps.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    var chiTietHoaDon = new CthoaDonDoanhNghiep
                    {
                        IdhoaDonDn = hoaDon.IdhoaDonDn,
                        IdgoiDichVu = idGoiDangKy,
                        DonGia = 0,
                        SoLuong = 1,
                        ThanhTien = 0,
                    };
                    _context.CthoaDonDoanhNghieps.Add(chiTietHoaDon);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Đăng ký dịch vụ thành công.";
                    return RedirectToAction("ThongBaoHoanTat", new { idHoaDon = hoaDon.IdhoaDonDn });
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    var innerException = dbEx.InnerException != null ? dbEx.InnerException.Message : "Không có thông tin chi tiết.";
                    TempData["ErrorMessage"] = $"Lỗi cơ sở dữ liệu: {innerException}";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Lỗi không xác định: {ex.Message} - {ex.StackTrace}";
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
            var hoaDonDichVu = _context.HoaDonDoanhNghieps
                                    .FirstOrDefault(h => h.IdhoaDonDn == idHoaDon);

            if (hoaDonDichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng dịch vụ.";
                return RedirectToAction("Index", "Home");
            }

            return View(hoaDonDichVu);
        }

    }
}
