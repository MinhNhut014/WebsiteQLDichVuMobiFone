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
    public class CthoaDonDichVusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CthoaDonDichVusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CthoaDonDichVus
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CthoaDonDichVus.Include(c => c.IdgoiDangKyDvkNavigation).Include(c => c.IdgoiDangKyNavigation).Include(c => c.IdhoaDonDvNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/CthoaDonDichVus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonDichVu = await _context.CthoaDonDichVus
                .Include(c => c.IdgoiDangKyDvkNavigation)
                .Include(c => c.IdgoiDangKyNavigation)
                .Include(c => c.IdhoaDonDvNavigation)
                .FirstOrDefaultAsync(m => m.IdcthoaDonDv == id);
            if (cthoaDonDichVu == null)
            {
                return NotFound();
            }

            return View(cthoaDonDichVu);
        }

        // GET: Admin/CthoaDonDichVus/Create
        public IActionResult Create()
        {
            ViewData["IdgoiDangKyDvk"] = new SelectList(_context.GoiDangKyDichVuKhacs, "IdgoiDangKy", "IdgoiDangKy");
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy");
            ViewData["IdhoaDonDv"] = new SelectList(_context.HoaDonDichVus, "IdhoaDonDv", "IdhoaDonDv");
            return View();
        }

        // POST: Admin/CthoaDonDichVus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdcthoaDonDv,IdhoaDonDv,IdgoiDangKy,IdgoiDangKyDvk,DonGia,SoLuong,ThanhTien")] CthoaDonDichVu cthoaDonDichVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cthoaDonDichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdgoiDangKyDvk"] = new SelectList(_context.GoiDangKyDichVuKhacs, "IdgoiDangKy", "IdgoiDangKy", cthoaDonDichVu.IdgoiDangKyDvk);
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy", cthoaDonDichVu.IdgoiDangKy);
            ViewData["IdhoaDonDv"] = new SelectList(_context.HoaDonDichVus, "IdhoaDonDv", "IdhoaDonDv", cthoaDonDichVu.IdhoaDonDv);
            return View(cthoaDonDichVu);
        }

        // GET: Admin/CthoaDonDichVus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonDichVu = await _context.CthoaDonDichVus.FindAsync(id);
            if (cthoaDonDichVu == null)
            {
                return NotFound();
            }
            ViewData["IdgoiDangKyDvk"] = new SelectList(_context.GoiDangKyDichVuKhacs, "IdgoiDangKy", "IdgoiDangKy", cthoaDonDichVu.IdgoiDangKyDvk);
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy", cthoaDonDichVu.IdgoiDangKy);
            ViewData["IdhoaDonDv"] = new SelectList(_context.HoaDonDichVus, "IdhoaDonDv", "IdhoaDonDv", cthoaDonDichVu.IdhoaDonDv);
            return View(cthoaDonDichVu);
        }

        // POST: Admin/CthoaDonDichVus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdcthoaDonDv,IdhoaDonDv,IdgoiDangKy,IdgoiDangKyDvk,DonGia,SoLuong,ThanhTien")] CthoaDonDichVu cthoaDonDichVu)
        {
            if (id != cthoaDonDichVu.IdcthoaDonDv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cthoaDonDichVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CthoaDonDichVuExists(cthoaDonDichVu.IdcthoaDonDv))
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
            ViewData["IdgoiDangKyDvk"] = new SelectList(_context.GoiDangKyDichVuKhacs, "IdgoiDangKy", "IdgoiDangKy", cthoaDonDichVu.IdgoiDangKyDvk);
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy", cthoaDonDichVu.IdgoiDangKy);
            ViewData["IdhoaDonDv"] = new SelectList(_context.HoaDonDichVus, "IdhoaDonDv", "IdhoaDonDv", cthoaDonDichVu.IdhoaDonDv);
            return View(cthoaDonDichVu);
        }

        // GET: Admin/CthoaDonDichVus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonDichVu = await _context.CthoaDonDichVus
                .Include(c => c.IdgoiDangKyDvkNavigation)
                .Include(c => c.IdgoiDangKyNavigation)
                .Include(c => c.IdhoaDonDvNavigation)
                .FirstOrDefaultAsync(m => m.IdcthoaDonDv == id);
            if (cthoaDonDichVu == null)
            {
                return NotFound();
            }

            return View(cthoaDonDichVu);
        }

        // POST: Admin/CthoaDonDichVus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cthoaDonDichVu = await _context.CthoaDonDichVus.FindAsync(id);
            if (cthoaDonDichVu != null)
            {
                _context.CthoaDonDichVus.Remove(cthoaDonDichVu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CthoaDonDichVuExists(int id)
        {
            return _context.CthoaDonDichVus.Any(e => e.IdcthoaDonDv == id);
        }
    }
}
