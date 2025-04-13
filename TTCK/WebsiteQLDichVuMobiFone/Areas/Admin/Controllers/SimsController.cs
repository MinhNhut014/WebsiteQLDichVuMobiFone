using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class SimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public void GetData()
        {
            var tenDangNhap = HttpContext.Session.GetString("nguoidung");

            if (!string.IsNullOrEmpty(tenDangNhap))
            {
                ViewBag.khachHang = _context.NguoiDungs.FirstOrDefault(k => k.TenDangNhap == tenDangNhap);
            }

            ViewBag.UserName = tenDangNhap;
            ViewBag.UserAvatar = HttpContext.Session.GetString("UserAvatar");
        }

        // GET: Admin/Sims
        public async Task<IActionResult> Index(int? idLoaiSo)
        {
            GetData();
            TempData.Remove("SuccessMessage");

            var query = _context.Sims
                .Include(s => s.IddichVuNavigation)
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .Include(s => s.SimGoiDangKies)
                .ThenInclude(sg => sg.IdgoiDangKyNavigation)
                .OrderByDescending(s => s.Idsim)
                .AsQueryable();

            if (idLoaiSo.HasValue)
            {
                query = query.Where(s => s.IdloaiSo == idLoaiSo);
            }

            ViewBag.TrangThaiSim = await _context.TrangThaiSims.ToListAsync();
            ViewBag.LoaiSim = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", idLoaiSo);

            return View(await query.ToListAsync());
        }

        // Cập nhật trạng thái SIM
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int trangthai)
        {
            var sim = await _context.Sims.FindAsync(id);
            if (sim == null) return NotFound();

            var trangThaiTonTai = await _context.TrangThaiSims.AnyAsync(t => t.IdtrangThaiSim == trangthai);
            if (!trangThaiTonTai) return BadRequest("Trạng thái không hợp lệ");

            sim.IdtrangThaiSim = trangthai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái SIM thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/Sims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            GetData();
            if (id == null) return NotFound();

            var sim = await _context.Sims
                .Include(s => s.IddichVuNavigation)
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.IdtrangThaiSimNavigation)
                .Include(s => s.SimGoiDangKies)
                .ThenInclude(sg => sg.IdgoiDangKyNavigation)
                .Include(s => s.SimGoiDangKyDichVuKhacs)
                .ThenInclude(sg => sg.IdgoiDangKyNavigation)
                .FirstOrDefaultAsync(m => m.Idsim == id);

            if (sim == null) return NotFound();

            // Tách danh sách gói đăng ký theo loại
            ViewBag.GoiDangKySim = _context.GoiDangKies
                .Select(g => new SelectListItem { Value = g.IdgoiDangKy.ToString(), Text = g.TenGoi })
                .ToList();

            ViewBag.GoiDangKyDichVuKhac = _context.GoiDangKyDichVuKhacs
                .Select(g => new SelectListItem { Value = g.IdgoiDangKy.ToString(), Text = g.TenGoi })
                .ToList();

            return View(sim);
        }


        // GET: Admin/Sims/Create
        public IActionResult Create()
        {
            GetData();
            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu");
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo");
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai");
            ViewData["GoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "TenGoi");
            return View();
        }

        // POST: Admin/Sims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idsim,IddichVu,SoThueBao,IdloaiSo,KhuVucHoaMang,PhiHoaMang,IdtrangThaiSim")] Sim sim, List<int> selectedGoiDangKy)
        {
            GetData();
            if (await _context.Sims.AnyAsync(g => g.SoThueBao == sim.SoThueBao))
            {
                ModelState.AddModelError("SoThueBao", "Số Thuê Bao này đã có rồi, vui lòng nhập tên khác.");
            }

            sim.IddichVu = 2;
            if (ModelState.IsValid)
            {
                _context.Add(sim);
                await _context.SaveChangesAsync();

                foreach (var goiId in selectedGoiDangKy)
                {
                    _context.SimGoiDangKies.Add(new SimGoiDangKy
                    {
                        Idsim = sim.Idsim,
                        IdgoiDangKy = goiId,
                        NgayDangKy = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai", sim.IdtrangThaiSim);
            ViewData["GoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "TenGoi");
            return View(sim);
        }

        // GET: Admin/Sims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            GetData();
            if (id == null) return NotFound();

            var sim = await _context.Sims
                .Include(s => s.SimGoiDangKies)
                .ThenInclude(sg => sg.IdgoiDangKyNavigation)
                .FirstOrDefaultAsync(s => s.Idsim == id);

            if (sim == null) return NotFound();

            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai", sim.IdtrangThaiSim);
            ViewData["GoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "TenGoi", sim.SimGoiDangKies.Select(sg => sg.IdgoiDangKy));
            return View(sim);
        }

        // POST: Admin/Sims/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idsim,IddichVu,SoThueBao,IdloaiSo,KhuVucHoaMang,PhiHoaMang,IdtrangThaiSim")] Sim sim, List<int> selectedGoiDangKy)
        {
            GetData();
            if (id != sim.Idsim) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    sim.IddichVu = 2;

                    var existingGoiDangKy = _context.SimGoiDangKies.Where(sg => sg.Idsim == sim.Idsim);
                    _context.SimGoiDangKies.RemoveRange(existingGoiDangKy);

                    foreach (var goiId in selectedGoiDangKy)
                    {
                        _context.SimGoiDangKies.Add(new SimGoiDangKy
                        {
                            Idsim = sim.Idsim,
                            IdgoiDangKy = goiId,
                            NgayDangKy = DateTime.Now
                        });
                    }

                    _context.Update(sim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SimExists(sim.Idsim)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IddichVu"] = new SelectList(_context.DichVus, "IddichVu", "TenDichVu", sim.IddichVu);
            ViewData["IdloaiSo"] = new SelectList(_context.LoaiSos, "IdloaiSo", "TenLoaiSo", sim.IdloaiSo);
            ViewData["IdtrangThaiSim"] = new SelectList(_context.TrangThaiSims, "IdtrangThaiSim", "TenTrangThai", sim.IdtrangThaiSim);
            ViewData["GoiDangKy"] = new SelectList(_context.GoiDangKies, "IdgoiDangKy", "TenGoi", selectedGoiDangKy);
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
        [HttpPost]
        public async Task<IActionResult> AddGoiDangKy(int idSim, int idGoiDangKy, string loaiGoi)
        {
            if (loaiGoi == "SimGoiDangKy")
            {
                // Kiểm tra xem gói đã tồn tại chưa
                var existing = await _context.SimGoiDangKies
                    .FirstOrDefaultAsync(sg => sg.Idsim == idSim && sg.IdgoiDangKy == idGoiDangKy);

                if (existing != null)
                {
                    TempData["ErrorMessage"] = "Gói đăng ký này đã tồn tại.";
                    return RedirectToAction("Details", new { id = idSim });
                }

                // Thêm gói đăng ký
                var newGoiDangKy = new SimGoiDangKy
                {
                    Idsim = idSim,
                    IdgoiDangKy = idGoiDangKy,
                    NgayDangKy = DateTime.Now
                };
                _context.SimGoiDangKies.Add(newGoiDangKy);
            }
            else if (loaiGoi == "SimGoiDangKyDichVuKhac")
            {
                // Kiểm tra xem gói đã tồn tại chưa
                var existing = await _context.SimGoiDangKyDichVuKhacs
                    .FirstOrDefaultAsync(sg => sg.Idsim == idSim && sg.IdgoiDangKy == idGoiDangKy);

                if (existing != null)
                {
                    TempData["ErrorMessage"] = "Gói đăng ký dịch vụ khác này đã tồn tại.";
                    return RedirectToAction("Details", new { id = idSim });
                }

                // Thêm gói đăng ký dịch vụ khác
                var newGoiDangKyKhac = new SimGoiDangKyDichVuKhac
                {
                    Idsim = idSim,
                    IdgoiDangKy = idGoiDangKy,
                    NgayDangKy = DateTime.Now
                };
                _context.SimGoiDangKyDichVuKhacs.Add(newGoiDangKyKhac);
            }
            else
            {
                TempData["ErrorMessage"] = "Loại gói đăng ký không hợp lệ.";
                return RedirectToAction("Details", new { id = idSim });
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm gói đăng ký thành công.";
            return RedirectToAction("Details", new { id = idSim });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveGoiDangKy(int idSim, int idGoiDangKy, string loaiGoi)
        {
            if (loaiGoi == "SimGoiDangKy")
            {
                // Tìm gói đăng ký
                var goiDangKy = await _context.SimGoiDangKies
                    .FirstOrDefaultAsync(sg => sg.Idsim == idSim && sg.IdgoiDangKy == idGoiDangKy);

                if (goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Gói đăng ký không tồn tại.";
                    return RedirectToAction("Details", new { id = idSim });
                }

                // Xóa gói đăng ký
                _context.SimGoiDangKies.Remove(goiDangKy);
            }
            else if (loaiGoi == "SimGoiDangKyDichVuKhac")
            {
                // Tìm gói đăng ký dịch vụ khác
                var goiDangKyKhac = await _context.SimGoiDangKyDichVuKhacs
                    .FirstOrDefaultAsync(sg => sg.Idsim == idSim && sg.IdgoiDangKy == idGoiDangKy);

                if (goiDangKyKhac == null)
                {
                    TempData["ErrorMessage"] = "Gói đăng ký dịch vụ khác không tồn tại.";
                    return RedirectToAction("Details", new { id = idSim });
                }

                // Xóa gói đăng ký dịch vụ khác
                _context.SimGoiDangKyDichVuKhacs.Remove(goiDangKyKhac);
            }
            else
            {
                TempData["ErrorMessage"] = "Loại gói đăng ký không hợp lệ.";
                return RedirectToAction("Details", new { id = idSim });
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa gói đăng ký thành công.";
            return RedirectToAction("Details", new { id = idSim });
        }

    }
}
