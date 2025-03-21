﻿using Microsoft.AspNetCore.Mvc;
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
                    IdgoiDangKy = goiChon.IdgoiDangKy,
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
            // Lấy thông tin SIM
            var sim = await _context.Sims
                .Include(s => s.IdloaiSoNavigation)
                .FirstOrDefaultAsync(s => s.Idsim == idSim);

            if (sim == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin SIM!";
                return RedirectToAction("ChonMua");
            }

            // Lấy thông tin gói đăng ký
            var goiDangKy = await _context.GoiDangKies
                .FirstOrDefaultAsync(g => g.IdgoiDangKy == idGoiDangKy);

            if (goiDangKy == null)
            {
                TempData["Error"] = "Gói đăng ký không hợp lệ!";
                return RedirectToAction("ChonMua");
            }

            // Lấy danh sách phương thức vận chuyển
            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens.ToListAsync();

            if (phuongThucVanChuyen == null || !phuongThucVanChuyen.Any())
            {
                TempData["Error"] = "Không có phương thức vận chuyển nào!";
                return RedirectToAction("ChonMua");
            }

            // Truyền dữ liệu qua ViewBag
            ViewBag.Sim = sim;
            ViewBag.GoiDaChon = goiDangKy;
            ViewBag.PhuongThucVanChuyen = phuongThucVanChuyen;

            // Tính tổng tiền ban đầu (xử lý null)
            decimal phiHoaMang = sim.PhiHoaMang ?? 0;
            decimal giaGoi = goiDangKy.GiaGoi ?? 0;
            decimal tongTien = phiHoaMang + giaGoi;

            // Truyền tổng tiền chưa bao gồm phí vận chuyển
            ViewBag.TongTien = tongTien;

            // Truyền phí vận chuyển (chưa tính)
            ViewBag.PhiVanChuyen = 0; // Mặc định chưa có phí vận chuyển, sẽ được tính sau khi người dùng chọn phương thức vận chuyển

            return View("DangKy");
        }

        [HttpPost]
        public async Task<IActionResult> TinhTongTien(int idSim, int idGoiDangKy, int phuongThucVanChuyenId)
        {
            // Lấy thông tin SIM
            var sim = await _context.Sims
                .FirstOrDefaultAsync(s => s.Idsim == idSim);

            if (sim == null)
            {
                return Json(new { success = false, message = "Không tìm thấy SIM!" });
            }

            // Lấy thông tin gói đăng ký
            var goiDangKy = await _context.GoiDangKies
                .FirstOrDefaultAsync(g => g.IdgoiDangKy == idGoiDangKy);

            if (goiDangKy == null)
            {
                return Json(new { success = false, message = "Không tìm thấy gói đăng ký!" });
            }

            // Lấy thông tin phí vận chuyển từ phương thức vận chuyển
            var phuongThucVanChuyen = await _context.PhuongThucVanChuyens
                .FirstOrDefaultAsync(ptvc => ptvc.IdphuongThucVc == phuongThucVanChuyenId);

            if (phuongThucVanChuyen == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phương thức vận chuyển!" });
            }

            // Tính tổng tiền mới
            decimal phiHoaMang = sim.PhiHoaMang ?? 0;
            decimal giaGoi = goiDangKy.GiaGoi ?? 0;
            decimal phiVanChuyen = phuongThucVanChuyen.GiaVanChuyen ?? 0;

            decimal tongTien = phiHoaMang + giaGoi + phiVanChuyen;

            return Json(new { success = true, tongTien });
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
