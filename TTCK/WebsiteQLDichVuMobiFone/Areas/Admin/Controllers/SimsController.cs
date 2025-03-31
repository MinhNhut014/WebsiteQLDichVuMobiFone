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
        public void GetData()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa bằng cách kiểm tra session "nguoidung"
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");

            if (!string.IsNullOrEmpty(tenDangNhap))
            {
                // Tìm người dùng từ cơ sở dữ liệu bằng tên đăng nhập đã lưu trong session
                ViewBag.khachHang = _context.NguoiDungs.FirstOrDefault(k => k.TenDangNhap == tenDangNhap);
            }
            // Truyền thông tin vào ViewData hoặc ViewBag
            ViewBag.UserName = tenDangNhap;
            ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
        }

        // GET: Admin/Sims
        public async Task<IActionResult> Index(int? idLoaiSo)
        {
            GetData();

            var query = _context.Sims
                .Include(s => s.GoiDangKyDiKemNavigation)
                .Include(s => s.IddichVuNavigation)
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .OrderByDescending(s => s.Idsim) // Sắp xếp mới nhất

                .AsQueryable();

            // Lọc theo loại SIM nếu có chọn
            if (idLoaiSo.HasValue)
            {
                query = query.Where(s => s.IdloaiSo == idLoaiSo);
            }

            // Load danh sách trạng thái SIM và loại SIM để hiển thị bộ lọc
            ViewBag.TrangThaiSim = await _context.TrangThaiSims.ToListAsync();
            ViewBag.LoaiSim = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", idLoaiSo);

            return View(await query.ToListAsync());
        }


        //cập nhật trạng thái sim
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangthai)
        {
            var sim = await _context.Sims.FindAsync(id); // Tìm SIM thay vì Hóa đơn SIM
            if (sim == null) return NotFound();

            // Kiểm tra xem trạng thái có tồn tại trong bảng TrangThaiSim không
            var trangThaiTonTai = await _context.TrangThaiSims.AnyAsync(t => t.IdtrangThaiSim == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            // Cập nhật trạng thái của SIM
            sim.IdtrangThaiSim = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái SIM thành công!";
            return RedirectToAction("Index");
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
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu");
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo");
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai");
            return View();
        }

        // POST: Admin/Sims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idsim,IddichVu,SoThueBao,IdloaiSo,KhuVucHoaMang,PhiHoaMang,IdtrangThaiSim")] Sim sim)
        {
            if (await _context.Sims.AnyAsync(g => g.SoThueBao == sim.SoThueBao))
            {
                ModelState.AddModelError("SoThueBao", "Số Thuê Bao này đã có rồi, vui lòng nhập tên khác.");
            }
            sim.IddichVu = 2;
            if (ModelState.IsValid)
            {
                _context.Add(sim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai", sim.IdtrangThaiSim);
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
                    sim.IddichVu = 2;
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
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sim = await _context.Sims.FindAsync(id);
            if (sim != null)
            {
                _context.Sims.Remove(sim);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool SimExists(int id)
        {
            return _context.Sims.Any(e => e.Idsim == id);
        }
    }
}
