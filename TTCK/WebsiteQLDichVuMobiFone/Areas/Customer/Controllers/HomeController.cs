using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Areas.Customer.ViewModels;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult dichvusim(string filters, string search)
        {
            var sims = _context.Sims.Include(s => s.IdloaiSoNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                sims = sims.Where(s => s.SoThueBao.Contains(search));
            }

            var selectedFilters = string.IsNullOrEmpty(filters) ? new List<string>() : filters.Split(',').ToList();

            if (selectedFilters.Any())
            {
                sims = sims.Where(s =>
                    selectedFilters.Contains(s.SoThueBao.Substring(0, 3)) ||
                    selectedFilters.Contains(s.IdloaiSoNavigation.TenLoaiSo)
                );
            }

            ViewBag.DauSos = _context.Sims.Select(s => s.SoThueBao.Substring(0, 3)).Distinct().ToList();
            ViewBag.LoaiThueBaos = _context.LoaiSos.Select(l => l.TenLoaiSo).Distinct().ToList();
            ViewBag.SelectedFilters = selectedFilters;
            ViewBag.Search = search;

            return View(sims.ToList());
        }
        [HttpPost]
        public IActionResult Sim_ChonMua(int id)
        {
            var sim = _context.Sims
                .Include(s => s.IdloaiSoNavigation)
                .Include(s => s.GoiDangKyDiKemNavigation)
                .FirstOrDefault(s => s.Idsim == id);

            if (sim == null)
            {
                return NotFound("SIM không tồn tại.");
            }

            var goiCuocList = _context.GoiDangKies
                .Where(g => g.GiaGoi == 90000 || g.GiaGoi == 135000 || g.GiaGoi == 159000)
                .ToList();

            var model = new SimViewModel
            {
                SimId = sim.Idsim,
                SoThueBao = sim.SoThueBao,
                LoaiSo = sim.IdloaiSoNavigation?.TenLoaiSo ?? "Không xác định",
                KhuVucHoaMang = sim.KhuVucHoaMang ?? "Toàn quốc",
                PhiHoaMang = sim.PhiHoaMang ?? 0,
                TenGoiCuoc = sim.GoiDangKyDiKemNavigation?.TenGoi ?? "Không có gói cước",
                GiaGoiCuoc = sim.GoiDangKyDiKemNavigation?.GiaGoi ?? 0,
                GoiCuocList = goiCuocList
            };

            return View(model);
        }

        public IActionResult ChonGoiCuoc(int simId, int goiCuocId)
        {
            var sim = _context.Sims
                .Include(s => s.GoiDangKyDiKemNavigation)
                .FirstOrDefault(s => s.Idsim == simId);
            if (sim == null) return NotFound("SIM không tồn tại.");

            var goiCuoc = _context.GoiDangKies.FirstOrDefault(g => g.IdgoiDangKy == goiCuocId);
            if (goiCuoc == null) return NotFound("Gói cước không tồn tại.");

            var model = new SimViewModel
            {
                SoThueBao = sim.SoThueBao,
                LoaiSo = sim.IdloaiSoNavigation?.TenLoaiSo ?? "Không xác định",
                KhuVucHoaMang = sim.KhuVucHoaMang ?? "Toàn quốc",
                PhiHoaMang = sim.PhiHoaMang ?? 0,
                TenGoiCuoc = goiCuoc.TenGoi,
                GiaGoiCuoc = goiCuoc.GiaGoi,
                GoiCuocId = goiCuoc.IdgoiDangKy,
                SimId = sim.Idsim
            };

            return View("Sim_ChonMua", model);
        }

        [HttpGet]
        public IActionResult Sim_DangKy(int simId, int goiCuocId)
        {
            var sim = _context.Sims
                .FirstOrDefault(s => s.Idsim == simId);

            var goiCuoc = _context.GoiDangKies
                .FirstOrDefault(g => g.IdgoiDangKy == goiCuocId);

            // Kiểm tra xem SIM hoặc Gói cước có tồn tại không
            if (sim == null || goiCuoc == null)
            {
                return RedirectToAction("Index", "Home"); // Hoặc trang lỗi
            }

            // Tạo ViewModel
            var viewModel = new SimDangKyViewModel
            {
                GoiCuocId = goiCuoc.IdgoiDangKy,
                SimId = sim.Idsim,
                SoThueBao = sim.SoThueBao,
                LoaiSo = sim.IdloaiSoNavigation.TenLoaiSo,
                TenGoiCuoc = goiCuoc.TenGoi,
                GiaGoiCuoc = goiCuoc.GiaGoi,
                PhiHoaMang = sim.PhiHoaMang ?? 0,
                GiaVanChuyen = 0, // Giả sử không có phí giao hàng cho demo
                PhuongThucVanChuyens = _context.PhuongThucVanChuyens.ToList()
            };

            return View(viewModel);
        }

    }
}
