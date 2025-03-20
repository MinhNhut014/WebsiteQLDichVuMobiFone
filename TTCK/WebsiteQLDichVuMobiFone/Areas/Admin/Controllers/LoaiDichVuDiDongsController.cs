using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoaiDichVuDiDongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiDichVuDiDongsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoaiDichVuDiDongs
        public async Task<IActionResult> Index()
        {
            var loaiDichVus = await _context.LoaiDichVuDiDongs.Include(l => l.IddichVuNavigation).ToListAsync();
            return View(loaiDichVus);
        }

        // GET: Admin/LoaiDichVuDiDongs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiDichVuDiDongs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoaiDichVu,IddichVu")] LoaiDichVuDiDong loaiDichVuDiDong)
        {
            // Gán ID Dịch Vụ cố định cho loại dịch vụ di động
            loaiDichVuDiDong.IddichVu = 1;

            // Kiểm tra xem tên loại dịch vụ đã tồn tại chưa
            if (_context.LoaiDichVuDiDongs.Any(l => l.TenLoaiDichVu == loaiDichVuDiDong.TenLoaiDichVu))
            {
                ModelState.AddModelError("TenLoaiDichVu", "Tên loại dịch vụ đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(loaiDichVuDiDong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(loaiDichVuDiDong);
        }



        // GET: Admin/LoaiDichVuDiDongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loaiDichVuDiDong = await _context.LoaiDichVuDiDongs.FindAsync(id);
            if (loaiDichVuDiDong == null) return NotFound();

            return View(loaiDichVuDiDong);
        }

        // POST: Admin/LoaiDichVuDiDongs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdloaiDichVu, TenLoaiDichVu")] LoaiDichVuDiDong loaiDichVuDiDong)
        {
            if (id != loaiDichVuDiDong.IdloaiDichVu) return NotFound();

            if (_context.LoaiDichVuDiDongs.Any(l => l.TenLoaiDichVu == loaiDichVuDiDong.TenLoaiDichVu && l.IdloaiDichVu != id))
            {
                ModelState.AddModelError("TenLoaiDichVu", "Tên loại dịch vụ đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    loaiDichVuDiDong.IddichVu = 1;
                    _context.Update(loaiDichVuDiDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LoaiDichVuDiDongs.Any(e => e.IdloaiDichVu == id))
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

            return View(loaiDichVuDiDong);
        }
        // GET: Admin/LoaiDichVuDiDongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loaiDichVuDiDong = await _context.LoaiDichVuDiDongs
                .Include(l => l.LoaiGoiDangKies) // Lấy danh sách các loại gói đăng ký liên quan
                .FirstOrDefaultAsync(m => m.IdloaiDichVu == id);

            if (loaiDichVuDiDong == null) return NotFound();

            return View(loaiDichVuDiDong);
        }


        // GET: Admin/LoaiDichVuDiDongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loaiDichVuDiDong = await _context.LoaiDichVuDiDongs.FindAsync(id);
            if (loaiDichVuDiDong == null) return NotFound();

            return View(loaiDichVuDiDong);
        }

        // POST: Admin/LoaiDichVuDiDongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiDichVuDiDong = await _context.LoaiDichVuDiDongs.FindAsync(id);
            if (loaiDichVuDiDong != null)
            {
                _context.LoaiDichVuDiDongs.Remove(loaiDichVuDiDong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
