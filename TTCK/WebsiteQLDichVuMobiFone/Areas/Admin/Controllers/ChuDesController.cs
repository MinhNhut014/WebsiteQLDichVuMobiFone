using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ChuDesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuDesController(ApplicationDbContext context)
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

        // GET: Admin/ChuDes
        public async Task<IActionResult> Index()
        {
            GetData();
            return View(await _context.ChuDes.ToListAsync());
        }

        // GET: Admin/ChuDes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            GetData();
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes
                .Include(m => m.TinTucs)
                .FirstOrDefaultAsync(m => m.IdchuDe == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        // GET: Admin/ChuDes/Create
        public IActionResult Create()
        {
            GetData();
            return View();
        }

        // POST: Admin/ChuDes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdchuDe,TenChuDe")] ChuDe chuDe)
        {
            GetData();
            if (await _context.ChuDes.AnyAsync(g => g.TenChuDe == chuDe.TenChuDe))
            {
                ModelState.AddModelError("TenChuDe", "Tên chủ đề này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(chuDe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chuDe);
        }

        // GET: Admin/ChuDes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            GetData();
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        // POST: Admin/ChuDes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdchuDe,TenChuDe")] ChuDe chuDe)
        {
            GetData();
            if (id != chuDe.IdchuDe)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chuDe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuDeExists(chuDe.IdchuDe))
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
            return View(chuDe);
        }

        // GET: Admin/ChuDes/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.ChuDes.FindAsync(id);
            if (gdv != null)
            {
                _context.ChuDes.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool ChuDeExists(int id)
        {
            return _context.ChuDes.Any(e => e.IdchuDe == id);
        }
    }
}
