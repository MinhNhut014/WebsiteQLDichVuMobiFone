using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class LoaiDichVuDiDongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiDichVuDiDongsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            GetData(); // Gọi hàm lấy dữ liệu
            base.OnActionExecuting(context);
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
        // GET: Admin/LoaiDichVuDiDongs
        public async Task<IActionResult> Index()
        {
            var loaiDichVus = await _context.LoaiDichVuDiDongs.Include(l => l.IddichVuNavigation).ToListAsync();
            return View(loaiDichVus);
        }

        // GET: Admin/LoaiDichVuDiDongs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiDichVuDiDongs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoaiDichVu,IddichVu")] LoaiDichVuDiDong loaiDichVuDiDong)
        {
            // Gán ID Dịch Vụ cố định cho loại dịch vụ di động
            loaiDichVuDiDong.IddichVu = 1;

            // Kiểm tra xem tên loại dịch vụ đã tồn tại chưa
            if (_context.LoaiDichVuDiDongs.Any(l => l.TenLoaiDichVu == loaiDichVuDiDong.TenLoaiDichVu))
            {
                ModelState.AddModelError("TenLoaiDichVu", "Tên loại dịch vụ đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(loaiDichVuDiDong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(loaiDichVuDiDong);
        }



        // GET: Admin/LoaiDichVuDiDongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loaiDichVuDiDong = await _context.LoaiDichVuDiDongs.FindAsync(id);
            if (loaiDichVuDiDong == null) return NotFound();

            return View(loaiDichVuDiDong);
        }

        // POST: Admin/LoaiDichVuDiDongs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdloaiDichVu, TenLoaiDichVu")] LoaiDichVuDiDong loaiDichVuDiDong)
        {
            if (id != loaiDichVuDiDong.IdloaiDichVu) return NotFound();

            if (_context.LoaiDichVuDiDongs.Any(l => l.TenLoaiDichVu == loaiDichVuDiDong.TenLoaiDichVu && l.IdloaiDichVu != id))
            {
                ModelState.AddModelError("TenLoaiDichVu", "Tên loại dịch vụ đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    loaiDichVuDiDong.IddichVu = 1;
                    _context.Update(loaiDichVuDiDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiDichVuDiDongs.Any(e => e.IdloaiDichVu == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(loaiDichVuDiDong);
        }
        // GET: Admin/LoaiDichVuDiDongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loaiDichVuDiDong = await _context.LoaiDichVuDiDongs
                .Include(l => l.LoaiGoiDangKies) // Lấy danh sách các loại gói đăng ký liên quan
                .FirstOrDefaultAsync(m => m.IdloaiDichVu == id);

            if (loaiDichVuDiDong == null) return NotFound();

            return View(loaiDichVuDiDong);
        }


        // GET: Admin/LoaiDichVuDiDongs/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.LoaiDichVuDiDongs.FindAsync(id);
            if (gdv != null)
            {
                _context.LoaiDichVuDiDongs.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
