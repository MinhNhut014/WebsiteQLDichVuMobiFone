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
    public class CthoaDonDoanhNghiepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CthoaDonDoanhNghiepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CthoaDonDoanhNghieps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CthoaDonDoanhNghieps.Include(c => c.IdgoiDichVuNavigation).Include(c => c.IdhoaDonDnNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/CthoaDonDoanhNghieps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonDoanhNghiep = await _context.CthoaDonDoanhNghieps
                .Include(c => c.IdgoiDichVuNavigation)
                .Include(c => c.IdhoaDonDnNavigation)
                .FirstOrDefaultAsync(m => m.IdcthoaDonDn == id);
            if (cthoaDonDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(cthoaDonDoanhNghiep);
        }

        // GET: Admin/CthoaDonDoanhNghieps/Create
        public IActionResult Create()
        {
            ViewData["IdgoiDichVu"] = new SelectList(_context.GoiDichVus, "IdgoiDichVu", "IdgoiDichVu");
            ViewData["IdhoaDonDn"] = new SelectList(_context.HoaDonDoanhNghieps, "IdhoaDonDn", "IdhoaDonDn");
            return View();
        }

        // POST: Admin/CthoaDonDoanhNghieps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdcthoaDonDn,IdhoaDonDn,IdgoiDichVu")] CthoaDonDoanhNghiep cthoaDonDoanhNghiep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cthoaDonDoanhNghiep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdgoiDichVu"] = new SelectList(_context.GoiDichVus, "IdgoiDichVu", "IdgoiDichVu", cthoaDonDoanhNghiep.IdgoiDichVu);
            ViewData["IdhoaDonDn"] = new SelectList(_context.HoaDonDoanhNghieps, "IdhoaDonDn", "IdhoaDonDn", cthoaDonDoanhNghiep.IdhoaDonDn);
            return View(cthoaDonDoanhNghiep);
        }

        // GET: Admin/CthoaDonDoanhNghieps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonDoanhNghiep = await _context.CthoaDonDoanhNghieps.FindAsync(id);
            if (cthoaDonDoanhNghiep == null)
            {
                return NotFound();
            }
            ViewData["IdgoiDichVu"] = new SelectList(_context.GoiDichVus, "IdgoiDichVu", "IdgoiDichVu", cthoaDonDoanhNghiep.IdgoiDichVu);
            ViewData["IdhoaDonDn"] = new SelectList(_context.HoaDonDoanhNghieps, "IdhoaDonDn", "IdhoaDonDn", cthoaDonDoanhNghiep.IdhoaDonDn);
            return View(cthoaDonDoanhNghiep);
        }

        // POST: Admin/CthoaDonDoanhNghieps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdcthoaDonDn,IdhoaDonDn,IdgoiDichVu")] CthoaDonDoanhNghiep cthoaDonDoanhNghiep)
        {
            if (id != cthoaDonDoanhNghiep.IdcthoaDonDn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cthoaDonDoanhNghiep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CthoaDonDoanhNghiepExists(cthoaDonDoanhNghiep.IdcthoaDonDn))
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
            ViewData["IdgoiDichVu"] = new SelectList(_context.GoiDichVus, "IdgoiDichVu", "IdgoiDichVu", cthoaDonDoanhNghiep.IdgoiDichVu);
            ViewData["IdhoaDonDn"] = new SelectList(_context.HoaDonDoanhNghieps, "IdhoaDonDn", "IdhoaDonDn", cthoaDonDoanhNghiep.IdhoaDonDn);
            return View(cthoaDonDoanhNghiep);
        }

        // GET: Admin/CthoaDonDoanhNghieps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonDoanhNghiep = await _context.CthoaDonDoanhNghieps
                .Include(c => c.IdgoiDichVuNavigation)
                .Include(c => c.IdhoaDonDnNavigation)
                .FirstOrDefaultAsync(m => m.IdcthoaDonDn == id);
            if (cthoaDonDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(cthoaDonDoanhNghiep);
        }

        // POST: Admin/CthoaDonDoanhNghieps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cthoaDonDoanhNghiep = await _context.CthoaDonDoanhNghieps.FindAsync(id);
            if (cthoaDonDoanhNghiep != null)
            {
                _context.CthoaDonDoanhNghieps.Remove(cthoaDonDoanhNghiep);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CthoaDonDoanhNghiepExists(int id)
        {
            return _context.CthoaDonDoanhNghieps.Any(e => e.IdcthoaDonDn == id);
        }
    }
}
