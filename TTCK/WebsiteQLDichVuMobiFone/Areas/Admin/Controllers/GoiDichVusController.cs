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
    public class GoiDichVusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoiDichVusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/GoiDichVus
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GoiDichVus.Include(g => g.IddichVuDnNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/GoiDichVus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDichVu = await _context.GoiDichVus
                .Include(g => g.IddichVuDnNavigation)
                .FirstOrDefaultAsync(m => m.IdgoiDichVu == id);
            if (goiDichVu == null)
            {
                return NotFound();
            }

            return View(goiDichVu);
        }

        // GET: Admin/GoiDichVus/Create
        public IActionResult Create()
        {
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "TenDichVu");
            return View();
        }

        // POST: Admin/GoiDichVus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //upload file
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\dichvu\\doanhnghiep\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile HinhAnh,[Bind("IdgoiDichVu,TenGoiDv,HinhAnh,MoTa,ThongTinChiTiet,IddichVuDn")] GoiDichVu goiDichVu)
        {
            if (ModelState.IsValid)
            {

                // ✨ Xử lý giữ nguyên xuống dòng và thụt đầu dòng
                goiDichVu.MoTa = goiDichVu.MoTa?.Replace("\n", "<br>");
                goiDichVu.ThongTinChiTiet = goiDichVu.ThongTinChiTiet?.Replace("\n", "<br>");

                //upload ảnh vào
                goiDichVu.HinhAnh = Upload(HinhAnh);

                _context.Add(goiDichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "TenDichVu", goiDichVu.IddichVuDn);
            return View(goiDichVu);
        }

        // GET: Admin/GoiDichVus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDichVu = await _context.GoiDichVus.FindAsync(id);
            if (goiDichVu == null)
            {
                return NotFound();
            }
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "IddichVuDn", goiDichVu.IddichVuDn);
            return View(goiDichVu);
        }

        // POST: Admin/GoiDichVus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdgoiDichVu,TenGoiDv,HinhAnh,MoTa,ThongTinChiTiet,IddichVuDn")] GoiDichVu goiDichVu)
        {
            if (id != goiDichVu.IdgoiDichVu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goiDichVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoiDichVuExists(goiDichVu.IdgoiDichVu))
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
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "IddichVuDn", goiDichVu.IddichVuDn);
            return View(goiDichVu);
        }

        // GET: Admin/GoiDichVus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDichVu = await _context.GoiDichVus
                .Include(g => g.IddichVuDnNavigation)
                .FirstOrDefaultAsync(m => m.IdgoiDichVu == id);
            if (goiDichVu == null)
            {
                return NotFound();
            }

            return View(goiDichVu);
        }

        // POST: Admin/GoiDichVus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goiDichVu = await _context.GoiDichVus.FindAsync(id);
            if (goiDichVu != null)
            {
                _context.GoiDichVus.Remove(goiDichVu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoiDichVuExists(int id)
        {
            return _context.GoiDichVus.Any(e => e.IdgoiDichVu == id);
        }
    }
}
