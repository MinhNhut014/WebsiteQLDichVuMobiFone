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
        public async Task<IActionResult> Index(int? idnhomDichVu)
        {
            var query = _context.DichVuDoanhNghieps
                .Include(d => d.IdnhomDichVuNavigation)
                .OrderByDescending(d => d.IddichVuDn) // Sắp xếp mới nhất
                .AsQueryable();

            // Lọc theo nhóm dịch vụ nếu có chọn
            if (idnhomDichVu.HasValue)
            {
                query = query.Where(d => d.IdnhomDichVu == idnhomDichVu.Value);
            }

            // Load danh sách nhóm dịch vụ để hiển thị bộ lọc
            ViewBag.NhomDichVu = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "TenNhom", idnhomDichVu);

            return View(await query.ToListAsync());
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
                .Include(d => d.GoiDichVus) // Lấy danh sách gói dịch vụ thuộc dịch vụ này
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
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "TenNhom");
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
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "TenNhom", dichVuDoanhNghiep.IdnhomDichVu);
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
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "TenNhom", dichVuDoanhNghiep.IdnhomDichVu);
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
            ViewData["IdnhomDichVu"] = new SelectList(_context.NhomDichVuDoanhNghieps, "IdnhomDichVu", "TenNhom", dichVuDoanhNghiep.IdnhomDichVu);
            return View(dichVuDoanhNghiep);
        }

        // GET: Admin/DichVuDoanhNghieps/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dv = await _context.DichVuDoanhNghieps.FindAsync(id);
            if (dv != null)
            {
                _context.DichVuDoanhNghieps.Remove(dv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DichVuDoanhNghiepExists(int id)
        {
            return _context.DichVuDoanhNghieps.Any(e => e.IddichVuDn == id);
        }
    }
}
