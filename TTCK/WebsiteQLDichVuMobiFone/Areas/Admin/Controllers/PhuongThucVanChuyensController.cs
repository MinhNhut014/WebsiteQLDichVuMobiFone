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
    public class PhuongThucVanChuyensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhuongThucVanChuyensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PhuongThucVanChuyens
        public async Task<IActionResult> Index()
        {
            return View(await _context.PhuongThucVanChuyens.ToListAsync());
        }

        // GET: Admin/PhuongThucVanChuyens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens
                .FirstOrDefaultAsync(m => m.IdphuongThucVc == id);
            if (phuongThucVanChuyen == null)
            {
                return NotFound();
            }

            return View(phuongThucVanChuyen);
        }

        // GET: Admin/PhuongThucVanChuyens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PhuongThucVanChuyens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdphuongThucVc,TenVanChuyen,MoTa,GiaVanChuyen")] PhuongThucVanChuyen phuongThucVanChuyen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phuongThucVanChuyen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phuongThucVanChuyen);
        }

        // GET: Admin/PhuongThucVanChuyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens.FindAsync(id);
            if (phuongThucVanChuyen == null)
            {
                return NotFound();
            }
            return View(phuongThucVanChuyen);
        }

        // POST: Admin/PhuongThucVanChuyens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdphuongThucVc,TenVanChuyen,MoTa,GiaVanChuyen")] PhuongThucVanChuyen phuongThucVanChuyen)
        {
            if (id != phuongThucVanChuyen.IdphuongThucVc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phuongThucVanChuyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhuongThucVanChuyenExists(phuongThucVanChuyen.IdphuongThucVc))
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
            return View(phuongThucVanChuyen);
        }

        // GET: Admin/PhuongThucVanChuyens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens
                .FirstOrDefaultAsync(m => m.IdphuongThucVc == id);
            if (phuongThucVanChuyen == null)
            {
                return NotFound();
            }

            return View(phuongThucVanChuyen);
        }

        // POST: Admin/PhuongThucVanChuyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens.FindAsync(id);
            if (phuongThucVanChuyen != null)
            {
                _context.PhuongThucVanChuyens.Remove(phuongThucVanChuyen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhuongThucVanChuyenExists(int id)
        {
            return _context.PhuongThucVanChuyens.Any(e => e.IdphuongThucVc == id);
        }
    }
}
