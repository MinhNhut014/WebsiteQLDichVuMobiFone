
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DichVuKhacController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVuKhacController(ApplicationDbContext context)
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
        public IActionResult Index(string searchTerm, List<int> selectedCategories, List<int> selectedProducts, int page = 1)
        {
            GetData();
            
            const int pageSize = 15; // Number of items per page

            // Lấy dữ liệu danh mục dịch vụ khác kèm sản phẩm dịch vụ
            var danhMucs = _context.LoaiDichVuKhacs
                .Include(dm => dm.SanPhamDichVuKhacs)
                .ToList();

            // Lọc sản phẩm theo điều kiện tìm kiếm
            var sanPhamQuery = _context.SanPhamDichVuKhacs.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                sanPhamQuery = sanPhamQuery.Where(sp => sp.TenSanPham.Contains(searchTerm));
            }

            if (selectedCategories != null && selectedCategories.Any())
            {
                sanPhamQuery = sanPhamQuery
                    .Where(sp => selectedCategories.Contains(sp.IdloaiDichVuKhac));
            }

            if (selectedProducts != null && selectedProducts.Any())
            {
                sanPhamQuery = sanPhamQuery
                    .Where(sp => selectedProducts.Contains(sp.IdsanPham));
            }

            // Pagination logic
            var totalItems = sanPhamQuery.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var paginatedSanPham = sanPhamQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedCategories = selectedCategories ?? new List<int>();
            ViewBag.SelectedProducts = selectedProducts ?? new List<int>();
            ViewBag.SanPhamDichVu = paginatedSanPham;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(danhMucs);
        }

        public IActionResult ChiTietSanPham(int id)
        {
            GetData();
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            ViewBag.SanPhamId = TempData["SanPhamId"] as int? ?? id;
            var sanPham = _context.SanPhamDichVuKhacs
                            .Include(sp => sp.GoiDangKyDichVuKhacs)
                            .FirstOrDefault(sp => sp.IdsanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            // Lấy các gói đăng ký liên quan
            ViewBag.GoiDangKyDichVuKhac = _context.GoiDangKyDichVuKhacs
                                                  .Where(g => g.IdsanPham == id)
                                                  .ToList();

            // Lấy các dịch vụ liên quan dựa theo loại dịch vụ
            ViewBag.DichVuLienQuan = _context.SanPhamDichVuKhacs
                                             .Where(sp => sp.IdloaiDichVuKhac == sanPham.IdloaiDichVuKhac && sp.IdsanPham != id)
                                             .ToList();

            return View(sanPham);
        }
        [HttpGet]
        public IActionResult DangKyDichVu(int id, int idsanpham)
        {
            GetData();
            // Kiểm tra người dùng đã đăng nhập hay chưa
            var nguoiDungId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(nguoiDungId))
            {
                // Nếu chưa đăng nhập, đặt thông báo vào ViewBag và chuyển hướng về trang Index
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để mua đăng ký.";
                TempData["SanPhamId"] = idsanpham; // Lưu lại ID sản phẩm để chuyển hướng sau khi đăng nhập
                return RedirectToAction("ChiTietSanPham", new { id = idsanpham });
            }
            // Tìm kiếm gói đăng ký trong cơ sở dữ liệu
            var goiDangKy = _context.GoiDangKyDichVuKhacs.FirstOrDefault(g => g.IdgoiDangKy == id);

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
                var goiDangKy = await _context.GoiDangKyDichVuKhacs.FindAsync(idGoiDangKy);
                if (goiDangKy == null)
                {
                    TempData["ErrorMessage"] = "Gói đăng ký không tồn tại.";
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
                        IdgoiDangKyDvk = idGoiDangKy,
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
