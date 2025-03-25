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
            if (!string.IsNullOrEmpty(searchTerm))
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

    }
}
