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
    public class DichVuDoanhNghiepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuDoanhNghiepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DichVuDoanhNghieps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DichVuDoanhNghieps.Include(d => d.IdnhomDichVuNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/DichVuDoanhNghieps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVuDoanhNghiep = await _context.DichVuDoanhNghieps
                .Include(d => d.IdnhomDichVuNavigation)
                .FirstOrDefaultAsync(m => m.IddichVuDn == id);
            if (dichVuDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(dichVuDoanhNghiep);
        }

        // GET: Admin/DichVuDoanhNghieps/Create
        public IActionResult Create()
        {
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "IdnhomDichVu");
            return View();
        }

        // POST: Admin/DichVuDoanhNghieps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IddichVuDn,TenDichVu,IdnhomDichVu")] DichVuDoanhNghiep dichVuDoanhNghiep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dichVuDoanhNghiep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "IdnhomDichVu", dichVuDoanhNghiep.IdnhomDichVu);
            return View(dichVuDoanhNghiep);
        }

        // GET: Admin/DichVuDoanhNghieps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVuDoanhNghiep = await _context.DichVuDoanhNghieps.FindAsync(id);
            if (dichVuDoanhNghiep == null)
            {
                return NotFound();
            }
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "IdnhomDichVu", dichVuDoanhNghiep.IdnhomDichVu);
            return View(dichVuDoanhNghiep);
        }

        // POST: Admin/DichVuDoanhNghieps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IddichVuDn,TenDichVu,IdnhomDichVu")] DichVuDoanhNghiep dichVuDoanhNghiep)
        {
            if (id != dichVuDoanhNghiep.IddichVuDn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dichVuDoanhNghiep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DichVuDoanhNghiepExists(dichVuDoanhNghiep.IddichVuDn))
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
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "IdnhomDichVu", dichVuDoanhNghiep.IdnhomDichVu);
            return View(dichVuDoanhNghiep);
        }

        // GET: Admin/DichVuDoanhNghieps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dichVuDoanhNghiep = await _context.DichVuDoanhNghieps
                .Include(d => d.IdnhomDichVuNavigation)
                .FirstOrDefaultAsync(m => m.IddichVuDn == id);
            if (dichVuDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(dichVuDoanhNghiep);
        }

        // POST: Admin/DichVuDoanhNghieps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dichVuDoanhNghiep = await _context.DichVuDoanhNghieps.FindAsync(id);
            if (dichVuDoanhNghiep != null)
            {
                _context.DichVuDoanhNghieps.Remove(dichVuDoanhNghiep);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DichVuDoanhNghiepExists(int id)
        {
            return _context.DichVuDoanhNghieps.Any(e => e.IddichVuDn == id);
        }
    }
}
