using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class NguoiDungsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungsController(ApplicationDbContext context)
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
        // GET: Admin/NguoiDungs
        public async Task<IActionResult> Index()
        {
            GetData(); // Gọi phương thức lấy dữ liệu khác (nếu có)

            // Lấy danh sách người dùng từ database
            var nguoiDungs = await _context.NguoiDungs.ToListAsync();

            return View(nguoiDungs);
        }

        // POST: Cập nhật quyền người dùng
        [HttpPost]
        public async Task<IActionResult> UpdateRole(int id, int quyen)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            // Giới hạn giá trị quyền (1 = Admin, 0 = Người dùng)
            user.Quyen = (quyen == 0 || quyen == 1) ? quyen : user.Quyen;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // POST: Cập nhật trạng thái người dùng
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangthai)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null) return NotFound();

            // Giới hạn trạng thái (1 = Hoạt động, 0 = Bị khóa)
            user.Trangthai = (trangthai == 0 || trangthai == 1) ? trangthai : user.Trangthai;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\user\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }

        // GET: Admin/NguoiDungs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.IdnguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: Admin/NguoiDungs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile AnhDaiDien,[Bind("IdnguoiDung,AnhDaiDien,HoTen,Cccd,Email,SoDienThoai,DiaChi,TenDangNhap,MatKhau,Quyen,Trangthai")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                nguoiDung.AnhDaiDien = Upload(AnhDaiDien);
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDungs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(nguoiDung);
        }

        // POST: Admin/NguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile? AnhDaiDien, int id, [Bind("IdnguoiDung,HoTen,Cccd,Email,SoDienThoai,DiaChi,TenDangNhap,MatKhau,Quyen,Trangthai")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.IdnguoiDung)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.NguoiDungs.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Nếu có ảnh mới thì cập nhật, nếu không giữ nguyên ảnh cũ
                    if (AnhDaiDien != null && AnhDaiDien.Length > 0)
                    {
                        nguoiDung.AnhDaiDien = Upload(AnhDaiDien);
                    }
                    else
                    {
                        nguoiDung.AnhDaiDien = user.AnhDaiDien;
                    }

                    _context.Entry(user).CurrentValues.SetValues(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.IdnguoiDung))
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
            return View(nguoiDung);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung != null)
            {
                _context.NguoiDungs.Remove(nguoiDung);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.IdnguoiDung == id);
        }
    }
}
