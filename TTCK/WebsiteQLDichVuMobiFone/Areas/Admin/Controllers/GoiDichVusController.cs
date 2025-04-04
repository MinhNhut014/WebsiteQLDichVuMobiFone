using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Data;
using WebsiteQLDichVuMobiFone.Filters;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class GoiDichVusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoiDichVusController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/GoiDichVus
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            GetData(); // Gọi hàm lấy dữ liệu
            base.OnActionExecuting(context);
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
        public async Task<IActionResult> Index()
        {
            TempData.Remove("SuccessMessage");
            var applicationDbContext = _context.GoiDichVus.Include(g => g.IddichVuDnNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Admin/GoiDichVus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDichVu = await _context.GoiDichVus
                .Include(g => g.IddichVuDnNavigation)
                .FirstOrDefaultAsync(m => m.IdgoiDichVu == id);
            if (goiDichVu == null)
            {
                return NotFound();
            }

            return View(goiDichVu);
        }

        // GET: Admin/GoiDichVus/Create
        public IActionResult Create()
        {
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "TenDichVu");
            return View();
        }

        // POST: Admin/GoiDichVus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //upload file
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\dichvu\\doanhnghiep\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile HinhAnh,[Bind("IdgoiDichVu,TenGoiDv,HinhAnh,MoTa,ThongTinChiTiet,IddichVuDn")] GoiDichVu goiDichVu)
        {
            if (await _context.GoiDichVus.AnyAsync(g => g.TenGoiDv == goiDichVu.TenGoiDv))
            {
                ModelState.AddModelError("TenGoiDV", "Tên gói dịch vụ này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {

                // ✨ Xử lý giữ nguyên xuống dòng và thụt đầu dòng
                goiDichVu.MoTa = goiDichVu.MoTa?.Replace("\n", "<br>");
                goiDichVu.ThongTinChiTiet = goiDichVu.ThongTinChiTiet?.Replace("\n", "<br>");

                //upload ảnh vào
                goiDichVu.HinhAnh = Upload(HinhAnh);

                _context.Add(goiDichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "TenDichVu", goiDichVu.IddichVuDn);
            return View(goiDichVu);
        }

        // GET: Admin/GoiDichVus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var goiDichVu = await _context.GoiDichVus.FindAsync(id);
            if (goiDichVu == null)
            {
                return NotFound();
            }
            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "TenDichVu", goiDichVu.IddichVuDn);
            return View(goiDichVu);
        }

        // POST: Admin/GoiDichVus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile? HinhAnh, int id, [Bind("IdgoiDichVu,TenGoiDv,MoTa,ThongTinChiTiet,IddichVuDn")] GoiDichVu goiDichVu)
        {
            if (id != goiDichVu.IdgoiDichVu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dv = await _context.GoiDichVus.FindAsync(id);
                    if (dv == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin mới từ form
                    dv.TenGoiDv = goiDichVu.TenGoiDv;
                    dv.MoTa = goiDichVu.MoTa;
                    dv.ThongTinChiTiet = goiDichVu.ThongTinChiTiet;
                    dv.IddichVuDn = goiDichVu.IddichVuDn;

                    // Nếu có ảnh mới, cập nhật ảnh
                    if (HinhAnh != null && HinhAnh.Length > 0)
                    {
                        dv.HinhAnh = Upload(HinhAnh);
                    }
                    // Nếu không có ảnh mới, giữ nguyên ảnh cũ (không cần gán lại)

                    _context.Update(dv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoiDichVuExists(goiDichVu.IdgoiDichVu))
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

            ViewData["IddichVuDn"] = new SelectList(_context.DichVuDoanhNghieps, "IddichVuDn", "TenDichVu", goiDichVu.IddichVuDn);
            return View(goiDichVu);
        }

        // GET: Admin/GoiDichVus/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.GoiDichVus.FindAsync(id);
            if (gdv != null)
            {
                _context.GoiDichVus.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool GoiDichVuExists(int id)
        {
            return _context.GoiDichVus.Any(e => e.IdgoiDichVu == id);
        }
    }
}
