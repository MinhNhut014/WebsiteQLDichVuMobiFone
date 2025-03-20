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
    public class BinhLuanBaiVietsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BinhLuanBaiVietsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BinhLuanBaiViets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BinhLuanBaiViets.Include(b => b.IdTinTucNavigation).Include(b => b.NguoiDung);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/BinhLuanBaiViets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binhLuanBaiViet = await _context.BinhLuanBaiViets
                .Include(b => b.IdTinTucNavigation)
                .Include(b => b.NguoiDung)
                .FirstOrDefaultAsync(m => m.IdbinhLuan == id);
            if (binhLuanBaiViet == null)
            {
                return NotFound();
            }

            return View(binhLuanBaiViet);
        }

        // GET: Admin/BinhLuanBaiViets/Create
        public IActionResult Create()
        {
            ViewData["IdTinTuc"] = new SelectList(_context.TinTucs, "IdTinTuc", "IdTinTuc");
            ViewData["NguoiDungId"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            return View();
        }

        // POST: Admin/BinhLuanBaiViets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdbinhLuan,IdTinTuc,HoTen,NoiDung,NgayBinhLuan,NguoiDungId")] BinhLuanBaiViet binhLuanBaiViet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(binhLuanBaiViet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTinTuc"] = new SelectList(_context.TinTucs, "IdTinTuc", "IdTinTuc", binhLuanBaiViet.IdTinTuc);
            ViewData["NguoiDungId"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", binhLuanBaiViet.NguoiDungId);
            return View(binhLuanBaiViet);
        }

        // GET: Admin/BinhLuanBaiViets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binhLuanBaiViet = await _context.BinhLuanBaiViets.FindAsync(id);
            if (binhLuanBaiViet == null)
            {
                return NotFound();
            }
            ViewData["IdTinTuc"] = new SelectList(_context.TinTucs, "IdTinTuc", "IdTinTuc", binhLuanBaiViet.IdTinTuc);
            ViewData["NguoiDungId"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", binhLuanBaiViet.NguoiDungId);
            return View(binhLuanBaiViet);
        }

        // POST: Admin/BinhLuanBaiViets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdbinhLuan,IdTinTuc,HoTen,NoiDung,NgayBinhLuan,NguoiDungId")] BinhLuanBaiViet binhLuanBaiViet)
        {
            if (id != binhLuanBaiViet.IdbinhLuan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(binhLuanBaiViet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BinhLuanBaiVietExists(binhLuanBaiViet.IdbinhLuan))
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
            ViewData["IdTinTuc"] = new SelectList(_context.TinTucs, "IdTinTuc", "IdTinTuc", binhLuanBaiViet.IdTinTuc);
            ViewData["NguoiDungId"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", binhLuanBaiViet.NguoiDungId);
            return View(binhLuanBaiViet);
        }

        // GET: Admin/BinhLuanBaiViets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binhLuanBaiViet = await _context.BinhLuanBaiViets
                .Include(b => b.IdTinTucNavigation)
                .Include(b => b.NguoiDung)
                .FirstOrDefaultAsync(m => m.IdbinhLuan == id);
            if (binhLuanBaiViet == null)
            {
                return NotFound();
            }

            return View(binhLuanBaiViet);
        }

        // POST: Admin/BinhLuanBaiViets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var binhLuanBaiViet = await _context.BinhLuanBaiViets.FindAsync(id);
            if (binhLuanBaiViet != null)
            {
                _context.BinhLuanBaiViets.Remove(binhLuanBaiViet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BinhLuanBaiVietExists(int id)
        {
            return _context.BinhLuanBaiViets.Any(e => e.IdbinhLuan == id);
        }
    }
}
