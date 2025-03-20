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
    public class HoaDonDoanhNghiepsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonDoanhNghiepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDonDoanhNghieps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HoaDonDoanhNghieps.Include(h => h.IdnguoiDungNavigation).Include(h => h.IdtrangThaiNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/HoaDonDoanhNghieps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDoanhNghiep = await _context.HoaDonDoanhNghieps
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonDn == id);
            if (hoaDonDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(hoaDonDoanhNghiep);
        }

        // GET: Admin/HoaDonDoanhNghieps/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai");
            return View();
        }

        // POST: Admin/HoaDonDoanhNghieps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdhoaDonDn,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenCongTy,SoDienThoaiCongTy,EmailCongTy,DiaChiCongTy")] HoaDonDoanhNghiep hoaDonDoanhNghiep)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonDoanhNghiep);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDoanhNghiep.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDoanhNghiep.IdtrangThai);
            return View(hoaDonDoanhNghiep);
        }

        // GET: Admin/HoaDonDoanhNghieps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDoanhNghiep = await _context.HoaDonDoanhNghieps.FindAsync(id);
            if (hoaDonDoanhNghiep == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDoanhNghiep.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDoanhNghiep.IdtrangThai);
            return View(hoaDonDoanhNghiep);
        }

        // POST: Admin/HoaDonDoanhNghieps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdhoaDonDn,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenCongTy,SoDienThoaiCongTy,EmailCongTy,DiaChiCongTy")] HoaDonDoanhNghiep hoaDonDoanhNghiep)
        {
            if (id != hoaDonDoanhNghiep.IdhoaDonDn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonDoanhNghiep);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonDoanhNghiepExists(hoaDonDoanhNghiep.IdhoaDonDn))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDoanhNghiep.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDoanhNghiep.IdtrangThai);
            return View(hoaDonDoanhNghiep);
        }

        // GET: Admin/HoaDonDoanhNghieps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDoanhNghiep = await _context.HoaDonDoanhNghieps
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonDn == id);
            if (hoaDonDoanhNghiep == null)
            {
                return NotFound();
            }

            return View(hoaDonDoanhNghiep);
        }

        // POST: Admin/HoaDonDoanhNghieps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDonDoanhNghiep = await _context.HoaDonDoanhNghieps.FindAsync(id);
            if (hoaDonDoanhNghiep != null)
            {
                _context.HoaDonDoanhNghieps.Remove(hoaDonDoanhNghiep);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonDoanhNghiepExists(int id)
        {
            return _context.HoaDonDoanhNghieps.Any(e => e.IdhoaDonDn == id);
        }
    }
}
