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
        public async Task<IActionResult> Index(string searchTerm, List<int> selectedCategories, int page = 1)
        {
            GetData();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            const int pageSize = 15; // Number of items per page

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

            // Pagination logic
            var totalItems = await goiDichVu.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var paginatedGoiDichVu = await goiDichVu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.GoiDichVu = paginatedGoiDichVu;
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(danhMucDichVu);
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
                newCode = $"HDDN-{DateTime.Now:yyyyMMdd}-{randomString}";
            }
            while (_context.HoaDonDoanhNghieps.Any(h => h.MaHoaDonDoanhNghiep == newCode)); // Kiểm tra trùng lặp

            return newCode;
        }
        public async Task<IActionResult> ChiTietDichVu(int id)
        {
            GetData();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
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
            // Kiểm tra người dùng đã đăng nhập hay chưa
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                // Nếu chưa đăng nhập, đặt thông báo vào TempData và chuyển hướng về trang trước đó
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đăng ký.";
                var referrer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referrer))
                {
                    return Redirect(referrer);
                }
                return RedirectToAction("Index");
            }

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
                hoaDon.MaHoaDonDoanhNghiep = GenerateUniqueInvoiceCode();
                hoaDon.TenCongTy = TenCongTy;
                hoaDon.SoDienThoaiCongTy = SoDienThoai;
                hoaDon.EmailCongTy = Email;
                hoaDon.DiaChiCongTy = DiaChi;
                hoaDon.IdnguoiDung = int.Parse(nguoiDungId);
                hoaDon.NgayDatHang = DateTime.Now;
                hoaDon.IdtrangThai = 1;

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
