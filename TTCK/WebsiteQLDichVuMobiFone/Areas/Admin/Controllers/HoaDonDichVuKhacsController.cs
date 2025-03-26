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
    public class HoaDonDichVuKhacsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonDichVuKhacsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDonDichVuKhacs
        public async Task<IActionResult> Index()
        {
            var hoaDonDichVus = await _context.HoaDonDichVus
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .Where(h => h.CthoaDonDichVus
                    .Any(ct => ct.IdgoiDangKyDvk != null))
                .Where(h => h.CthoaDonDichVus
                    .All(ct => ct.IdgoiDangKy == null)) // Chỉ giữ lại các hóa đơn không chứa IdgoiDangKyDvk
                .OrderByDescending(h => h.NgayDatHang)
                .ToListAsync();

            return View(hoaDonDichVus);
        }

        // GET: Admin/HoaDonDichVuKhacs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDichVu = await _context.HoaDonDichVus
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonDv == id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }

            return View(hoaDonDichVu);
        }

        // GET: Admin/HoaDonDichVuKhacs/Create
        public IActionResult Create()
        {
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung");
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai");
            return View();
        }

        // POST: Admin/HoaDonDichVuKhacs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdhoaDonDv,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email,DiaChi")] HoaDonDichVu hoaDonDichVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDonDichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDichVu.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDichVu.IdtrangThai);
            return View(hoaDonDichVu);
        }

        // GET: Admin/HoaDonDichVuKhacs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDichVu = await _context.HoaDonDichVus.FindAsync(id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDichVu.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDichVu.IdtrangThai);
            return View(hoaDonDichVu);
        }

        // POST: Admin/HoaDonDichVuKhacs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdhoaDonDv,IdnguoiDung,NgayDatHang,TongTien,IdtrangThai,TenKhachHang,SoDienThoai,Email,DiaChi")] HoaDonDichVu hoaDonDichVu)
        {
            if (id != hoaDonDichVu.IdhoaDonDv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDonDichVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonDichVuExists(hoaDonDichVu.IdhoaDonDv))
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
            ViewData["IdnguoiDung"] = new SelectList(_context.NguoiDungs, "IdnguoiDung", "IdnguoiDung", hoaDonDichVu.IdnguoiDung);
            ViewData["IdtrangThai"] = new SelectList(_context.TrangThaiDonHangs, "IdtrangThai", "IdtrangThai", hoaDonDichVu.IdtrangThai);
            return View(hoaDonDichVu);
        }

        // GET: Admin/HoaDonDichVuKhacs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDonDichVu = await _context.HoaDonDichVus
                .Include(h => h.IdnguoiDungNavigation)
                .Include(h => h.IdtrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.IdhoaDonDv == id);
            if (hoaDonDichVu == null)
            {
                return NotFound();
            }

            return View(hoaDonDichVu);
        }

        // POST: Admin/HoaDonDichVuKhacs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDonDichVu = await _context.HoaDonDichVus.FindAsync(id);
            if (hoaDonDichVu != null)
            {
                _context.HoaDonDichVus.Remove(hoaDonDichVu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonDichVuExists(int id)
        {
            return _context.HoaDonDichVus.Any(e => e.IdhoaDonDv == id);
        }
    }
}
