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
            if (!string.IsNullOrWhiteSpace(searchTerm))
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
        public IActionResult DangKyDichVu(int id)
        {
            GetData();
            // Kiểm tra ID gói dịch vụ được truyền vào
            Console.WriteLine("ID được truyền vào: " + id);

            // Tìm kiếm gói đăng ký trong cơ sở dữ liệu
            var goiDangKy = _context.GoiDangKies.FirstOrDefault(g => g.IdgoiDangKy == id);

            // Kiểm tra gói đăng ký có tồn tại không
            if (goiDangKy == null)
            {
                TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                return RedirectToAction("Index");  // Điều hướng về trang chủ hoặc trang khác phù hợp
            }

            ViewBag.GoiDaChon = goiDangKy;
            ViewBag.TongTien = goiDangKy.GiaGoi;
            return View();
        }

        // Xử lý hoàn tất đăng ký dịch vụ
        [HttpPost]
        public async Task<IActionResult> HoanTatDangKyDichVu(HoaDonDichVu hoaDon, int idGoiDangKy, [FromForm] string SoDienThoai)
        {
            GetData();

            // Kiểm tra đăng nhập
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            // Kiểm tra số điện thoại trống
            if (string.IsNullOrEmpty(SoDienThoai))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập số điện thoại.";
                return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
            }

            try
            {
                // Kiểm tra số thuê bao trong bảng SIM
                var sim = await _context.Sims.FirstOrDefaultAsync(s => s.SoThueBao == SoDienThoai);
                if (sim == null || sim.IdtrangThaiSim != 3)  // Trạng thái SIM kích hoạt (IdtrangThaiSim == 3)
                {
                    TempData["ErrorMessage"] = "SIM không tồn tại hoặc chưa kích hoạt.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                // Kiểm tra gói đăng ký
                var goiDangKy = await _context.GoiDangKies.FindAsync(idGoiDangKy);
                if (goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Gói dịch vụ không tồn tại.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                // Kiểm tra SIM đã đăng ký gói này chưa
                bool daDangKy = await _context.CthoaDonDichVus.AnyAsync(ct =>
                    ct.IdgoiDangKy == idGoiDangKy && ct.IdhoaDonDvNavigation.SoDienThoai == SoDienThoai);

                if (daDangKy)
                {
                    TempData["ErrorMessage"] = "Số thuê bao này đã đăng ký gói dịch vụ này rồi.";
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }

                // Thiết lập thông tin hóa đơn
                hoaDon.SoDienThoai = SoDienThoai;
                hoaDon.IdnguoiDung = int.Parse(nguoiDungId);
                hoaDon.NgayDatHang = DateTime.Now;
                hoaDon.IdtrangThai = 1; // Trạng thái "Chờ xử lý"
                hoaDon.TongTien = goiDangKy.GiaGoi;


                // Bắt đầu transaction
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Thêm hóa đơn vào bảng HoaDonDichVu
                    _context.HoaDonDichVus.Add(hoaDon);
                    await _context.SaveChangesAsync();

                    // Thêm chi tiết hóa đơn vào bảng CthoaDonDichVu
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

                    // Cập nhật gói đăng ký vào SIM đã kiểm tra
                    sim.GoiDangKyDiKem = idGoiDangKy; // Cột IdgoiDangKy trong bảng SIM
                    _context.Sims.Update(sim);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Đăng ký dịch vụ thành công.";
                    return RedirectToAction("ThongBaoHoanTat", new { idHoaDon = hoaDon.IdhoaDonDv });
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Lỗi cơ sở dữ liệu: " + dbEx.InnerException?.Message;
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Lỗi không xác định: " + ex.Message;
                    return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
                return RedirectToAction("DangKyDichVu", new { id = idGoiDangKy });
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
