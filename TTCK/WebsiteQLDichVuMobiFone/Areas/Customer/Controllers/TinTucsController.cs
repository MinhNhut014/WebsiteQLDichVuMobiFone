using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TinTucsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TinTucsController(ApplicationDbContext context)
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
        public async Task<IActionResult> Index(int page = 1, int chuDeId = 0)
        {
            GetData();
            int pageSize = 20; // Số tin tức trên mỗi trang
            var query = _context.TinTucs.AsQueryable();

            // Lọc theo chủ đề nếu có
            if (chuDeId != 0)
            {
                query = query.Where(t => t.IdTheLoai == chuDeId);
            }

            // Tính tổng số trang
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy danh sách tin tức theo trang
            var tinTucs = await query
                .OrderByDescending(t => t.NgayDang)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lưu dữ liệu vào ViewBag để dùng trong View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.ChuDeList = await _context.ChuDes.ToListAsync();

            return View(tinTucs);
        }
        public async Task<IActionResult> ChiTiet(int id)
        {
            GetData();
            var tinTuc = await _context.TinTucs
                .Include(t => t.IdTheLoaiNavigation) // Lấy thông tin chủ đề
                .Include(t => t.BinhLuanBaiViets) // Lấy danh sách bình luận
                .ThenInclude(bl => bl.NguoiDung) // Lấy thông tin người dùng bình luận
                .FirstOrDefaultAsync(t => t.IdTinTuc == id);

            if (tinTuc == null)
            {
                return NotFound();
            }
            // Tăng lượt xem lên 1
            tinTuc.LuotXem += 1;
            _context.SaveChanges(); // Lưu vào database

            return View(tinTuc);
        }
        [HttpPost]
        public async Task<IActionResult> ThemBinhLuan(int idTinTuc, string noiDung)
        {
            GetData(); // Gọi lại để lấy dữ liệu người dùng

            // Lấy thông tin từ ViewBag do GetData() đã cập nhật
            var user = ViewBag.khachHang as NguoiDung;

            if (user == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để bình luận.";
                return RedirectToAction("ChiTiet", new { id = idTinTuc });
            }

            var binhLuan = new BinhLuanBaiViet
            {
                IdTinTuc = idTinTuc,
                NguoiDungId = user.IdnguoiDung,
                HoTen = user.HoTen, // Lấy họ tên từ tài khoản đã đăng nhập
                NoiDung = noiDung,
                NgayBinhLuan = DateTime.Now
            };

            _context.BinhLuanBaiViets.Add(binhLuan);
            await _context.SaveChangesAsync();

            return RedirectToAction("ChiTiet", new { id = idTinTuc });
        }

    }
}
