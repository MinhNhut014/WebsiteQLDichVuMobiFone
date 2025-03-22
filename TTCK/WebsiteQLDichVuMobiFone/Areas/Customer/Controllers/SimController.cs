using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Areas.Customer.ViewModels;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SimController(ApplicationDbContext context)
        {
            _context = context;
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

        public IActionResult ChonMua(int id, int? maGoi)
        {
            // Lấy thông tin SIM cần mua
            var sim = _context.Sims.Include(s => s.IdloaiSoNavigation).FirstOrDefault(s => s.Idsim == id);
            if (sim == null)
            {
                TempData["Error"] = "SIM không tồn tại!";
                return RedirectToAction("dichvusim");
            }

            // Lấy danh sách gói cước mặc định
            var goiCuoc = _context.GoiDangKies
                .Where(g => (g.GiaGoi == 90000 || g.GiaGoi == 135000 || g.GiaGoi == 159000) && g.ThoiHan == "30")
                .ToList();

            if (goiCuoc == null || goiCuoc.Count == 0)
            {
                TempData["Error"] = "Không có gói cước khả dụng!";
                return RedirectToAction("dichvusim");
            }

            ViewBag.GoiDangKy = goiCuoc;
            ViewBag.Sim = sim;

            // Xóa dữ liệu cũ trong TempData trước khi thêm mới
            TempData.Remove("Cart");

            // Gói cước mặc định hoặc gói được chọn
            var goiChon = maGoi.HasValue
                ? goiCuoc.FirstOrDefault(g => g.IdgoiDangKy == maGoi.Value)
                : goiCuoc.FirstOrDefault();

            if (goiChon != null)
            {
                // Tạo đối tượng Cart mới
                var cartItem = new CartViewModel
                {
                    Idsim = sim.Idsim,
                    SoThueBao = sim.SoThueBao,
                    LoaiThueBao = sim.IdloaiSoNavigation?.TenLoaiSo ?? "Không xác định",
                    MaGoi = goiChon.IdgoiDangKy,
                    TenGoi = goiChon.TenGoi,
                    GiaGoi = goiChon.GiaGoi
                };

                // Chuyển đối tượng thành danh sách để đảm bảo cấu trúc dữ liệu
                var cart = new List<CartViewModel> { cartItem };

                // Lưu dữ liệu mới vào TempData
                TempData["Cart"] = Newtonsoft.Json.JsonConvert.SerializeObject(cart);
                TempData.Keep("Cart"); // Giữ TempData sống sót sau redirect
            }

            return View();
        }


        /// chức năng đăng ksy đơn hàng sim
        // Action chuyển qua trang đăng ký SIM
        [HttpGet]
        public async Task<IActionResult> DangKy(int idSim, int idGoiDangKy)
        {
            var sim = await _context.Sims
                .Include(s => s.IdloaiSoNavigation)
                .FirstOrDefaultAsync(s => s.Idsim == idSim);

            var goiDangKy = await _context.GoiDangKies.FirstOrDefaultAsync(g => g.IdgoiDangKy == idGoiDangKy);

            if (sim == null || goiDangKy == null)
            {
                TempData["Error"] = "SIM hoặc gói đăng ký không hợp lệ!";
                return RedirectToAction("ChonMua");
            }

            ViewBag.Sim = sim;
            ViewBag.GoiDangKy = goiDangKy;

            return View("DangKySim");
        }

        [HttpPost]
        public async Task<IActionResult> LuuThongTinDangKy(HoaDonSim hoaDonSim, int idSim, int idGoiDangKy)
        {
            try
            {
                hoaDonSim.NgayDatHang = DateTime.Now;
                hoaDonSim.IdtrangThai = 1;
                _context.HoaDonSims.Add(hoaDonSim);
                await _context.SaveChangesAsync();

                var cthdSim = new CthoaDonSim
                {
                    IdhoaDonSim = hoaDonSim.IdhoaDonSim,
                    Idsim = idSim,
                    IdgoiDangKy = idGoiDangKy,
                    DonGia = hoaDonSim.TongTien,
                    SoLuong = 1,
                    ThanhTien = hoaDonSim.TongTien
                };
                _context.CthoaDonSims.Add(cthdSim);
                await _context.SaveChangesAsync();

                return RedirectToAction("HoanTatDangKy", new { id = hoaDonSim.IdhoaDonSim });
            }
            catch
            {
                TempData["Error"] = "Đăng ký thất bại! Vui lòng thử lại.";
                return RedirectToAction("DangKy", new { idSim, idGoiDangKy });
            }
        }

        public async Task<IActionResult> HoanTatDangKy(int id)
        {
            var hoaDonSim = await _context.HoaDonSims
                .Include(h => h.CthoaDonSims)
                .ThenInclude(c => c.IdsimNavigation)
                .Include(h => h.CthoaDonSims)
                .ThenInclude(c => c.IdgoiDangKyNavigation)
                .FirstOrDefaultAsync(h => h.IdhoaDonSim == id);

            if (hoaDonSim == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("dichvusim");
            }

            return View(hoaDonSim);
        }

    }
}
