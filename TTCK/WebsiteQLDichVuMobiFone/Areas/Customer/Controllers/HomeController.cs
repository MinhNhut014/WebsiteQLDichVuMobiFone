using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            // Lọc tìm kiếm theo số thuê bao
            if (!string.IsNullOrEmpty(search))
            {
                sims = sims.Where(s => s.SoThueBao.Contains(search));
            }

            // Lọc đầu số và loại thuê bao
            var selectedFilters = string.IsNullOrEmpty(filters) ? new List<string>() : filters.Split(',').ToList();

            if (selectedFilters.Any())
            {
                sims = sims.Where(s =>
                    selectedFilters.Contains(s.SoThueBao.Substring(0, 3)) ||               // Đầu số
                    selectedFilters.Contains(s.IdloaiSoNavigation.TenLoaiSo)             // Loại thuê bao
                );
            }

            // Lấy danh sách đầu số và loại thuê bao từ CSDL
            var dauSos = _context.Sims.Select(s => s.SoThueBao.Substring(0, 3)).Distinct().ToList();
            var loaiThueBaos = _context.LoaiSos.Select(l => l.TenLoaiSo).Distinct().ToList();

            ViewBag.DauSos = dauSos;
            ViewBag.LoaiThueBaos = loaiThueBaos;
            ViewBag.SelectedFilters = selectedFilters;
            ViewBag.Search = search;

            return View(sims.ToList());
        }
    }
}
