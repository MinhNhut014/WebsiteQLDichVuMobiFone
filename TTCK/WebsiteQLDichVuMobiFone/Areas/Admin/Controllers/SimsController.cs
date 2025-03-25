using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Sims
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sims.Include(s => s.GoiDangKyDiKemNavigation).Include(s => s.IddichVuNavigation).Include(s => s.IdloaiSoNavigation).Include(s => s.IdtrangThaiSimNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Sims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sim = await _context.Sims
                .Include(s => s.GoiDangKyDiKemNavigation)
                .Include(s => s.IddichVuNavigation)
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .FirstOrDefaultAsync(m => m.Idsim == id);
            if (sim == null)
            {
                return NotFound();
            }

            return View(sim);
        }

        // GET: Admin/Sims/Create
        public IActionResult Create()
        {
            ViewData["GoiDangKyDiKem"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "IdgoiDangKy");
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu");
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "IdloaiSo");
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "IdtrangThaiSim");
            return View();
        }

        // POST: Admin/Sims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idsim,IddichVu,SoThueBao,IdloaiSo,KhuVucHoaMang,PhiHoaMang,IdtrangThaiSim")] Sim sim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "IddichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "IdloaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "IdtrangThaiSim", sim.IdtrangThaiSim);
            return View(sim);
        }

        // GET: Admin/Sims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sim = await _context.Sims.FindAsync(id);
            if (sim == null)
            {
                return NotFound();
            }

            ViewData["GoiDangKyDiKem"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "TenGoi", sim.GoiDangKyDiKem);
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai", sim.IdtrangThaiSim);

            return View(sim);
        }

        // POST: Admin/Sims/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idsim,IddichVu,SoThueBao,IdloaiSo,KhuVucHoaMang,PhiHoaMang,IdtrangThaiSim,GoiDangKyDiKem")] Sim sim)
        {
            if (id != sim.Idsim)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra nếu người dùng chọn "-- Không chọn --"
                    if (sim.GoiDangKyDiKem == 0)
                    {
                        sim.GoiDangKyDiKem = null; // Set null khi không chọn gói đăng ký đi kèm
                    }

                    _context.Update(sim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Sims.Any(e => e.Idsim == sim.Idsim))
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

            ViewData["GoiDangKyDiKem"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "TenGoi", sim.GoiDangKyDiKem);
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai", sim.IdtrangThaiSim);

            return View(sim);
        }

        // GET: Admin/Sims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sim = await _context.Sims
                .Include(s => s.GoiDangKyDiKemNavigation)
                .Include(s => s.IddichVuNavigation)
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .FirstOrDefaultAsync(m => m.Idsim == id);
            if (sim == null)
            {
                return NotFound();
            }

            return View(sim);
        }

        // POST: Admin/Sims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sim = await _context.Sims.FindAsync(id);
            if (sim != null)
            {
                _context.Sims.Remove(sim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SimExists(int id)
        {
            return _context.Sims.Any(e => e.Idsim == id);
        }
    }
}
