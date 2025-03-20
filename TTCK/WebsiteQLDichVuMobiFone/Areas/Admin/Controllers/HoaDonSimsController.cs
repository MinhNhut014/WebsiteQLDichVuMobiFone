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
    public class HoaDonSimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonSimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDonSims
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HoaDonSims.Include(h => h.IdnguoiDungNavigation).Include(h => h.IdphuongThucVcNavigation).Include(h => h.IdtrangThaiNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/HoaDonSims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonSim = await _context.HoaDonSims
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdphuongThucVcNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonSim == id);
            if (hoaDonSim == null)
            {
                return NotFound();
            }

            return View(hoaDonSim);
        }

        // GET: Admin/HoaDonSims/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc");
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai");
            return View();
        }

        // POST: Admin/HoaDonSims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdhoaDonSim,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email,DiaDiemNhan,PhuongThucThanhToan,IdphuongThucVc")] HoaDonSim hoaDonSim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonSim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonSim.IdnguoiDung);
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc", hoaDonSim.IdphuongThucVc);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonSim.IdtrangThai);
            return View(hoaDonSim);
        }

        // GET: Admin/HoaDonSims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonSim = await _context.HoaDonSims.FindAsync(id);
            if (hoaDonSim == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonSim.IdnguoiDung);
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc", hoaDonSim.IdphuongThucVc);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonSim.IdtrangThai);
            return View(hoaDonSim);
        }

        // POST: Admin/HoaDonSims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdhoaDonSim,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email,DiaDiemNhan,PhuongThucThanhToan,IdphuongThucVc")] HoaDonSim hoaDonSim)
        {
            if (id != hoaDonSim.IdhoaDonSim)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonSim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonSimExists(hoaDonSim.IdhoaDonSim))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonSim.IdnguoiDung);
            ViewData["IdphuongThucVc"] = new SelectList(_context.PhuongThucVanChuyens, "IdphuongThucVc", "IdphuongThucVc", hoaDonSim.IdphuongThucVc);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonSim.IdtrangThai);
            return View(hoaDonSim);
        }

        // GET: Admin/HoaDonSims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonSim = await _context.HoaDonSims
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdphuongThucVcNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonSim == id);
            if (hoaDonSim == null)
            {
                return NotFound();
            }

            return View(hoaDonSim);
        }

        // POST: Admin/HoaDonSims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDonSim = await _context.HoaDonSims.FindAsync(id);
            if (hoaDonSim != null)
            {
                _context.HoaDonSims.Remove(hoaDonSim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonSimExists(int id)
        {
            return _context.HoaDonSims.Any(e => e.IdhoaDonSim == id);
        }
    }
}
