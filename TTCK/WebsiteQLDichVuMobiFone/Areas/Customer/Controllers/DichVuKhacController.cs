
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DichVuKhacController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuKhacController(ApplicationDbContext context)
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
        public IActionResult Index(string searchTerm, List<int> selectedCategories, List<int> selectedProducts)
        {
            GetData();
            // Lấy dữ liệu danh mục dịch vụ khác kèm sản phẩm dịch vụ
            var danhMucs = _context.LoaiDichVuKhacs
                .Include(dm => dm.SanPhamDichVuKhacs)
                .ToList();

            // Lọc sản phẩm theo điều kiện tìm kiếm
            var sanPhamQuery = _context.SanPhamDichVuKhacs.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                sanPhamQuery = sanPhamQuery.Where(sp => sp.TenSanPham.Contains(searchTerm));
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                sanPhamQuery = sanPhamQuery
                    .Where(sp => selectedCategories.Contains(sp.IdloaiDichVuKhac));
            }

            if (selectedProducts != null && selectedProducts.Any())
            {
                sanPhamQuery = sanPhamQuery
                    .Where(sp => selectedProducts.Contains(sp.IdsanPham));
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SelectedProducts = selectedProducts ?? new List<int>();
            ViewBag.SanPhamDichVu = sanPhamQuery.ToList();

            return View(danhMucs);
        }
    }
}
