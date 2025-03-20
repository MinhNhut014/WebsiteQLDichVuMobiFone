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
    public class CthoaDonSimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CthoaDonSimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CthoaDonSims
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CthoaDonSims.Include(c => c.IdgoiDangKyNavigation).Include(c => c.IdhoaDonSimNavigation).Include(c => c.IdsimNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/CthoaDonSims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonSim = await _context.CthoaDonSims
                .Include(c => c.IdgoiDangKyNavigation)
                .Include(c => c.IdhoaDonSimNavigation)
                .Include(c => c.IdsimNavigation)
                .FirstOrDefaultAsync(m => m.IdcthoaDonSim == id);
            if (cthoaDonSim == null)
            {
                return NotFound();
            }

            return View(cthoaDonSim);
        }

        // GET: Admin/CthoaDonSims/Create
        public IActionResult Create()
        {
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy");
            ViewData["IdhoaDonSim"] = new SelectList(_context.HoaDonSims, "IdhoaDonSim", "IdhoaDonSim");
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim");
            return View();
        }

        // POST: Admin/CthoaDonSims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdcthoaDonSim,IdhoaDonSim,Idsim,IdgoiDangKy,DonGia,SoLuong,ThanhTien")] CthoaDonSim cthoaDonSim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cthoaDonSim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy", cthoaDonSim.IdgoiDangKy);
            ViewData["IdhoaDonSim"] = new SelectList(_context.HoaDonSims, "IdhoaDonSim", "IdhoaDonSim", cthoaDonSim.IdhoaDonSim);
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim", cthoaDonSim.Idsim);
            return View(cthoaDonSim);
        }

        // GET: Admin/CthoaDonSims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonSim = await _context.CthoaDonSims.FindAsync(id);
            if (cthoaDonSim == null)
            {
                return NotFound();
            }
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy", cthoaDonSim.IdgoiDangKy);
            ViewData["IdhoaDonSim"] = new SelectList(_context.HoaDonSims, "IdhoaDonSim", "IdhoaDonSim", cthoaDonSim.IdhoaDonSim);
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim", cthoaDonSim.Idsim);
            return View(cthoaDonSim);
        }

        // POST: Admin/CthoaDonSims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdcthoaDonSim,IdhoaDonSim,Idsim,IdgoiDangKy,DonGia,SoLuong,ThanhTien")] CthoaDonSim cthoaDonSim)
        {
            if (id != cthoaDonSim.IdcthoaDonSim)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cthoaDonSim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CthoaDonSimExists(cthoaDonSim.IdcthoaDonSim))
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
            ViewData["IdgoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy", cthoaDonSim.IdgoiDangKy);
            ViewData["IdhoaDonSim"] = new SelectList(_context.HoaDonSims, "IdhoaDonSim", "IdhoaDonSim", cthoaDonSim.IdhoaDonSim);
            ViewData["Idsim"] = new SelectList(_context.Sims, "Idsim", "Idsim", cthoaDonSim.Idsim);
            return View(cthoaDonSim);
        }

        // GET: Admin/CthoaDonSims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoaDonSim = await _context.CthoaDonSims
                .Include(c => c.IdgoiDangKyNavigation)
                .Include(c => c.IdhoaDonSimNavigation)
                .Include(c => c.IdsimNavigation)
                .FirstOrDefaultAsync(m => m.IdcthoaDonSim == id);
            if (cthoaDonSim == null)
            {
                return NotFound();
            }

            return View(cthoaDonSim);
        }

        // POST: Admin/CthoaDonSims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cthoaDonSim = await _context.CthoaDonSims.FindAsync(id);
            if (cthoaDonSim != null)
            {
                _context.CthoaDonSims.Remove(cthoaDonSim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CthoaDonSimExists(int id)
        {
            return _context.CthoaDonSims.Any(e => e.IdcthoaDonSim == id);
        }
    }
}
