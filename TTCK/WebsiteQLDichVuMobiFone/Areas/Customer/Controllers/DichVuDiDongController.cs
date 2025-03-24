using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;
using WebsiteQLDichVuMobiFone.Models.ViewModels;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DichVuDiDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuDiDongController(ApplicationDbContext context)
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
        public async Task<IActionResult> Index(string searchTerm, List<int> selectedCategories)
        {
            GetData();
            // Lấy danh sách danh mục dịch vụ di động cùng với loại gói đăng ký
            var danhMucDichVu = _context.LoaiDichVuDiDongs
                                        .Select(dm => new LoaiDichVuDiDong
                                        {
                                            IdloaiDichVu = dm.IdloaiDichVu,
                                            TenLoaiDichVu = dm.TenLoaiDichVu,
                                            LoaiGoiDangKies = dm.LoaiGoiDangKies.ToList()
                                        }).ToList();

            // Lấy danh sách gói đăng ký
            var goiDangKy = _context.GoiDangKies.AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                goiDangKy = goiDangKy.Where(g => g.TenGoi.Contains(searchTerm));
            }

            // Lọc theo danh mục được chọn
            if (selectedCategories != null && selectedCategories.Any())
            {
                goiDangKy = goiDangKy.Where(g => selectedCategories.Contains(g.IdloaiGoi));
            }

            ViewBag.GoiDangKy = goiDangKy.ToList();
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SearchTerm = searchTerm;

            return View(danhMucDichVu);
        }
        public async Task<IActionResult> ChiTietDichVu(int id)
        {
            GetData();
            var goiDangKy = await _context.GoiDangKies
                .FirstOrDefaultAsync(g => g.IdgoiDangKy == id);

            if (goiDangKy == null)
            {
                return NotFound();
            }

            var goiCuocTuongTu = await _context.GoiDangKies
                .Where(g => g.IdloaiGoi == goiDangKy.IdloaiGoi && g.IdgoiDangKy != id)
                .Take(3)
                .ToListAsync();

            ViewBag.GoiCuocTuongTu = goiCuocTuongTu;

            return View(goiDangKy);
        }
        [HttpGet]
        public async Task<IActionResult> DangKyDichVu(int id)
        {
            GetData();
            var goiDangKy = await _context.GoiDangKies.FindAsync(id);

            if (goiDangKy == null)
            {
                TempData["Error"] = "Không tìm thấy gói dịch vụ.";
                return RedirectToAction("DanhSachDichVu");
            }

            ViewBag.GoiDaChon = goiDangKy; // Chỉ định rõ biến lưu gói dịch vụ đã chọn
            ViewBag.DanhSachGoiDichVu = _context.GoiDangKies.ToList(); // Danh sách tất cả gói dịch vụ (nếu cần)
            ViewBag.TongTien = goiDangKy.GiaGoi;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HoanTatDangKyDichVu(HoaDonDichVu hoaDon, int idGoiDangKy)
        {
            GetData();
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            // Kiểm tra số điện thoại trong bảng SIM
            var sim = await _context.Sims.FirstOrDefaultAsync(s => s.SoThueBao == hoaDon.SoDienThoai);
            if (sim == null)
            {
                TempData["ErrorMessage"] = "SIM không tồn tại.";
                return RedirectToAction("DangKyDichVu", new { idGoiDangKy });
            }

            if (sim.IdtrangThaiSim != 3)
            {
                TempData["ErrorMessage"] = "SIM chưa kích hoạt.";
                return RedirectToAction("DangKyDichVu", new { idGoiDangKy });
            }

            hoaDon.IdnguoiDung = int.Parse(nguoiDungId);
            hoaDon.NgayDatHang = DateTime.Now;
            hoaDon.IdtrangThai = 1; // Trạng thái "Chờ xử lý"
            hoaDon.DiaChi = null; // Địa chỉ mặc định là không có

            var goiDangKy = await _context.GoiDangKies.FindAsync(idGoiDangKy);
            if (goiDangKy == null)
            {
                TempData["ErrorMessage"] = "Gói dịch vụ không hợp lệ.";
                return RedirectToAction("DangKyDichVu", new { idGoiDangKy });
            }

            hoaDon.TongTien = goiDangKy.GiaGoi;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.HoaDonDichVus.Add(hoaDon);
                await _context.SaveChangesAsync();

                var chiTietHoaDon = new CthoaDonDichVu
                {
                    IdhoaDonDv = hoaDon.IdhoaDonDv,
                    IdgoiDangKy = idGoiDangKy,
                    DonGia = goiDangKy.GiaGoi,
                    SoLuong = 1,
                    ThanhTien = goiDangKy.GiaGoi
                };

                _context.CthoaDonDichVus.Add(chiTietHoaDon);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Đăng ký dịch vụ thành công.";
                return RedirectToAction("ThongBaoHoanTat", new { idHoaDon = hoaDon.IdhoaDonDv });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("DangKyDichVu", new { idGoiDangKy });
            }
        }
        public IActionResult ThongBaoHoanTat(int idHoaDon)
        {
            GetData();
            var hoaDonDichVu = _context.HoaDonDichVus
                                    .FirstOrDefault(h => h.IdhoaDonDv == idHoaDon);

            if (hoaDonDichVu == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng dịch vụ.";
                return RedirectToAction("Index", "Home");
            }

            return View(hoaDonDichVu);
        }

    }
}
