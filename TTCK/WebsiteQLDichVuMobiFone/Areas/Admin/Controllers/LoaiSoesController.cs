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
    public class LoaiSoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiSoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoaiSoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoaiSos.ToListAsync());
        }

        // GET: Admin/LoaiSoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSo = await _context.LoaiSos
                .FirstOrDefaultAsync(m => m.IdloaiSo == id);
            if (loaiSo == null)
            {
                return NotFound();
            }

            return View(loaiSo);
        }

        // GET: Admin/LoaiSoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiSoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdloaiSo,TenLoaiSo")] LoaiSo loaiSo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiSo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSo);
        }

        // GET: Admin/LoaiSoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSo = await _context.LoaiSos.FindAsync(id);
            if (loaiSo == null)
            {
                return NotFound();
            }
            return View(loaiSo);
        }

        // POST: Admin/LoaiSoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdloaiSo,TenLoaiSo")] LoaiSo loaiSo)
        {
            if (id != loaiSo.IdloaiSo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSoExists(loaiSo.IdloaiSo))
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
            return View(loaiSo);
        }

        // GET: Admin/LoaiSoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSo = await _context.LoaiSos
                .FirstOrDefaultAsync(m => m.IdloaiSo == id);
            if (loaiSo == null)
            {
                return NotFound();
            }

            return View(loaiSo);
        }

        // POST: Admin/LoaiSoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiSo = await _context.LoaiSos.FindAsync(id);
            if (loaiSo != null)
            {
                _context.LoaiSos.Remove(loaiSo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiSoExists(int id)
        {
            return _context.LoaiSos.Any(e => e.IdloaiSo == id);
        }
    }
}
