using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GoiDangKiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoiDangKiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/GoiDangKies
        public async Task<IActionResult> Index(int? idLoaiDichVu, int? idLoaiGoi)
        {
            // Lọc các gói theo dịch vụ di động
            var goiDangKies = _context.GoiDangKies
                .Include(g => g.IdloaiGoiNavigation)
                .ThenInclude(lg => lg.IdloaiDichVuNavigation)
                .AsQueryable();

            if (idLoaiDichVu.HasValue)
            {
                goiDangKies = goiDangKies.Where(g => g.IdloaiGoiNavigation.IdloaiDichVu == idLoaiDichVu);
            }

            if (idLoaiGoi.HasValue)
            {
                goiDangKies = goiDangKies.Where(g => g.IdloaiGoi == idLoaiGoi);
            }

            // Đổ dữ liệu vào dropdown
            ViewData["LoaiDichVus"] = new SelectList(_context.LoaiDichVuDiDongs, "IdloaiDichVu", "TenLoaiDichVu");
            ViewData["LoaiGoiDangKies"] = new SelectList(_context.LoaiGoiDangKies, "IdloaiGoi", "TenLoaiGoi");

            var result = await goiDangKies.OrderByDescending(b => b.IdgoiDangKy).ToListAsync();
            return View(result);
        }
        public async Task<JsonResult> GetLoaiGoiByDichVu(int idLoaiDichVu)
        {
            var loaiGoiList = await _context.LoaiGoiDangKies
                .Where(lg => lg.IdloaiDichVu == idLoaiDichVu)
                .Select(lg => new { lg.IdloaiGoi, lg.TenLoaiGoi })
                .ToListAsync();

            return Json(loaiGoiList);
        }


        // GET: Admin/GoiDangKies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDangKy = await _context.GoiDangKies
                .Include(g => g.IdloaiGoiNavigation)
                .FirstOrDefaultAsync(m => m.IdgoiDangKy == id);
            if (goiDangKy == null)
            {
                return NotFound();
            }

            return View(goiDangKy);
        }

        // GET: Admin/GoiDangKies/Create
        public IActionResult Create()
        {
            ViewData["IdloaiGoi"] = new SelectList(_context.LoaiGoiDangKies, "IdloaiGoi", "TenLoaiGoi");
            return View();
        }

        // POST: Admin/GoiDangKies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdgoiDangKy,TenGoi,GiaGoi,ThoiHan,TinhNang,IdloaiGoi,ThongTinGoi,ThongTinChiTiet")] GoiDangKy goiDangKy)
        {
            if (ModelState.IsValid)
            {
                // ✨ Xử lý giữ nguyên xuống dòng và thụt đầu dòng
                goiDangKy.ThongTinGoi = goiDangKy.ThongTinGoi?.Replace("\n", "<br>");
                goiDangKy.ThongTinChiTiet = goiDangKy.ThongTinChiTiet?.Replace("\n", "<br>");

                _context.Add(goiDangKy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdloaiGoi"] = new SelectList(_context.LoaiGoiDangKies, "IdloaiGoi", "TenLoaiGoi", goiDangKy.IdloaiGoi);
            return View(goiDangKy);
        }

        // GET: Admin/GoiDangKies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDangKy = await _context.GoiDangKies.FindAsync(id);
            if (goiDangKy == null)
            {
                return NotFound();
            }
            ViewData["IdloaiGoi"] = new SelectList(_context.LoaiGoiDangKies, "IdloaiGoi", "TenLoaiGoi", goiDangKy.IdloaiGoi);
            return View(goiDangKy);
        }

        // POST: Admin/GoiDangKies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdgoiDangKy,TenGoi,GiaGoi,ThoiHan,TinhNang,IdloaiGoi,ThongTinGoi,ThongTinChiTiet")] GoiDangKy goiDangKy)
        {
            if (id != goiDangKy.IdgoiDangKy)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ✨ Xử lý giữ nguyên xuống dòng và thụt đầu dòng
                    goiDangKy.ThongTinGoi = goiDangKy.ThongTinGoi?.Replace("\n", "<br>");
                    goiDangKy.ThongTinChiTiet = goiDangKy.ThongTinChiTiet?.Replace("\n", "<br>");

                    _context.Update(goiDangKy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoiDangKyExists(goiDangKy.IdgoiDangKy))
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
            ViewData["IdloaiGoi"] = new SelectList(_context.LoaiGoiDangKies, "IdloaiGoi", "TenLoaiGoi", goiDangKy.IdloaiGoi);
            return View(goiDangKy);
        }

        // GET: Admin/GoiDangKies/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goidk = await _context.GoiDangKies.FindAsync(id);
            if (goidk != null)
            {
                _context.GoiDangKies.Remove(goidk);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool GoiDangKyExists(int id)
        {
            return _context.GoiDangKies.Any(e => e.IdgoiDangKy == id);
        }
    }
}
