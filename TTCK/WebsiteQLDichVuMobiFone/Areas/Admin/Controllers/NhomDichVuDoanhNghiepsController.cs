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
    public class NhomDichVuDoanhNghiepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhomDichVuDoanhNghiepsController(ApplicationDbContext context)
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
        // GET: Admin/NhomDichVuDoanhNghieps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.NhomDichVuDoanhNghieps.Include(n => n.IddichVuNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/NhomDichVuDoanhNghieps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhomDichVuDoanhNghiep = await _context.NhomDichVuDoanhNghieps
                .Include(n => n.IddichVuNavigation)
                .Include(n => n.DichVuDoanhNghieps) // Lấy danh sách dịch vụ doanh nghiệp thuộc nhóm này
                .FirstOrDefaultAsync(m => m.IdnhomDichVu == id);

            if (nhomDichVuDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(nhomDichVuDoanhNghiep);
        }


        // GET: Admin/NhomDichVuDoanhNghieps/Create
        public IActionResult Create()
        {
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu");
            return View();
        }

        // POST: Admin/NhomDichVuDoanhNghieps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdnhomDichVu,TenNhom")] NhomDichVuDoanhNghiep nhomDichVuDoanhNghiep)
        {
            if (await _context.NhomDichVuDoanhNghieps.AnyAsync(g => g.TenNhom == nhomDichVuDoanhNghiep.TenNhom))
            {
                ModelState.AddModelError("TenNhom", "Tên dịch vụ này đã có rồi, vui lòng nhập tên khác.");
            }
            nhomDichVuDoanhNghiep.IddichVu = 3; // Gán tự động ID Dịch Vụ = 3

            if (ModelState.IsValid)
            {
                _context.Add(nhomDichVuDoanhNghiep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhomDichVuDoanhNghiep);
        }

        // GET: Admin/NhomDichVuDoanhNghieps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhomDichVuDoanhNghiep = await _context.NhomDichVuDoanhNghieps.FindAsync(id);
            if (nhomDichVuDoanhNghiep == null)
            {
                return NotFound();
            }
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu", nhomDichVuDoanhNghiep.IddichVu);
            return View(nhomDichVuDoanhNghiep);
        }

        // POST: Admin/NhomDichVuDoanhNghieps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdnhomDichVu,IddichVu,TenNhom")] NhomDichVuDoanhNghiep nhomDichVuDoanhNghiep)
        {
            if (id != nhomDichVuDoanhNghiep.IdnhomDichVu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhomDichVuDoanhNghiep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhomDichVuDoanhNghiepExists(nhomDichVuDoanhNghiep.IdnhomDichVu))
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
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu", nhomDichVuDoanhNghiep.IddichVu);
            return View(nhomDichVuDoanhNghiep);
        }

        // GET: Admin/NhomDichVuDoanhNghieps/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhomdn = await _context.NhomDichVuDoanhNghieps.FindAsync(id);
            if (nhomdn != null)
            {
                _context.NhomDichVuDoanhNghieps.Remove(nhomdn);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NhomDichVuDoanhNghiepExists(int id)
        {
            return _context.NhomDichVuDoanhNghieps.Any(e => e.IdnhomDichVu == id);
        }
    }
}
