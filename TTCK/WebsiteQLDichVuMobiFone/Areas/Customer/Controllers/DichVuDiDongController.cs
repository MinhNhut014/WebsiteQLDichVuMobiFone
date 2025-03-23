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
        public async Task<IActionResult> Index(string searchTerm, List<int> selectedCategories)
        {
            GetData();
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
            if (!string.IsNullOrEmpty(searchTerm))
            {
                goiDangKy = goiDangKy.Where(g => g.TenGoi.Contains(searchTerm));
            }

            // Lọc theo danh mục được chọn
            if (selectedCategories != null && selectedCategories.Any())
            {
                goiDangKy = goiDangKy.Where(g => selectedCategories.Contains(g.IdloaiGoi));
            }

            ViewBag.GoiDangKy = goiDangKy.ToList();
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SearchTerm = searchTerm;

            return View(danhMucDichVu);
        }

    }
}
