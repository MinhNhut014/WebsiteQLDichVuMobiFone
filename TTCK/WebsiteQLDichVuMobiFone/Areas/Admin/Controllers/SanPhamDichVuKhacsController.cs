using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class SanPhamDichVuKhacsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamDichVuKhacsController(ApplicationDbContext context)
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
        // GET: Admin/SanPhamDichVuKhacs
        // GET: Admin/SanPhamDichVuKhacs
        public async Task<IActionResult> Index(int? idLoaiDichVuKhac)
        {
            var query = _context.SanPhamDichVuKhacs
                .Include(s => s.IdloaiDichVuKhacNavigation)
                .AsQueryable();

            // Lọc theo loại dịch vụ nếu có chọn
            if (idLoaiDichVuKhac.HasValue)
            {
                query = query.Where(s => s.IdloaiDichVuKhac == idLoaiDichVuKhac);
            }

            // Load danh sách loại dịch vụ khác để hiển thị bộ lọc
            ViewBag.LoaiDichVuKhac = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", idLoaiDichVuKhac);

            return View(await query.ToListAsync());
        }

        // GET: Admin/SanPhamDichVuKhacs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPhamDichVuKhac = await _context.SanPhamDichVuKhacs
                .Include(s => s.IdloaiDichVuKhacNavigation)
                .Include(s => s.GoiDangKyDichVuKhacs)
                .FirstOrDefaultAsync(m => m.IdsanPham == id);
            if (sanPhamDichVuKhac == null)
            {
                return NotFound();
            }

            return View(sanPhamDichVuKhac);
        }

        // GET: Admin/SanPhamDichVuKhacs/Create
        public IActionResult Create()
        {
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu");
            return View();
        }

        //upload file
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\dichvu\\dichvukhac\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }

        // POST: Admin/SanPhamDichVuKhacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile HinhAnh, [Bind("IdsanPham,TenSanPham,HinhAnh,MoTa,ThongTinChiTiet,IdloaiDichVuKhac")] SanPhamDichVuKhac sanPhamDichVuKhac)
        {
            if (await _context.SanPhamDichVuKhacs.AnyAsync(g => g.TenSanPham == sanPhamDichVuKhac.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {
                // ✨ Xử lý giữ nguyên xuống dòng và thụt đầu dòng
                sanPhamDichVuKhac.MoTa = sanPhamDichVuKhac.MoTa?.Replace("\n", "<br>");
                sanPhamDichVuKhac.ThongTinChiTiet = sanPhamDichVuKhac.ThongTinChiTiet?.Replace("\n", "<br>");

                //upload ảnh vào
                sanPhamDichVuKhac.HinhAnh = Upload(HinhAnh);
                _context.Add(sanPhamDichVuKhac);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", sanPhamDichVuKhac.IdloaiDichVuKhac);
            return View(sanPhamDichVuKhac);
        }

        // GET: Admin/SanPhamDichVuKhacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPhamDichVuKhac = await _context.SanPhamDichVuKhacs.FindAsync(id);
            if (sanPhamDichVuKhac == null)
            {
                return NotFound();
            }
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", sanPhamDichVuKhac.IdloaiDichVuKhac);
            return View(sanPhamDichVuKhac);
        }

        // POST: Admin/SanPhamDichVuKhacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile? HinhAnh, int id, [Bind("IdsanPham,TenSanPham,HinhAnh,MoTa,ThongTinChiTiet,IdloaiDichVuKhac")] SanPhamDichVuKhac sanPhamDichVuKhac)
        {
            if (id != sanPhamDichVuKhac.IdsanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dv = await _context.SanPhamDichVuKhacs.FindAsync(id);
                    if (dv == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin mới từ form
                    dv.TenSanPham = sanPhamDichVuKhac.TenSanPham;
                    dv.MoTa = sanPhamDichVuKhac.MoTa;
                    dv.ThongTinChiTiet = sanPhamDichVuKhac.ThongTinChiTiet;
                    dv.IdloaiDichVuKhac = sanPhamDichVuKhac.IdloaiDichVuKhac;

                    // Nếu có ảnh mới, cập nhật ảnh
                    if (HinhAnh != null && HinhAnh.Length > 0)
                    {
                        dv.HinhAnh = Upload(HinhAnh);
                    }
                    // Nếu không có ảnh mới, giữ nguyên ảnh cũ (không cần gán lại)

                    _context.Update(dv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamDichVuKhacExists(sanPhamDichVuKhac.IdsanPham))
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
            ViewData["IdloaiDichVuKhac"] = new SelectList(_context.LoaiDichVuKhacs, "IdloaiDichVuKhac", "TenLoaiDichVu", sanPhamDichVuKhac.IdloaiDichVuKhac);
            return View(sanPhamDichVuKhac);
        }

        // GET: Admin/SanPhamDichVuKhacs/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.SanPhamDichVuKhacs.FindAsync(id);
            if (gdv != null)
            {
                _context.SanPhamDichVuKhacs.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool SanPhamDichVuKhacExists(int id)
        {
            return _context.SanPhamDichVuKhacs.Any(e => e.IdsanPham == id);
        }
    }
}
