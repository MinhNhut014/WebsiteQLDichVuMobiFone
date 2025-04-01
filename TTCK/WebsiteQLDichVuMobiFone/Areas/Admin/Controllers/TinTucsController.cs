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
    public class TinTucsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TinTucsController(ApplicationDbContext context)
        {
            _context = context;
        }
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
        // GET: Admin/TinTucs
        public async Task<IActionResult> Index(int? idTheLoai)
        {
            var query = _context.TinTucs
                .Include(t => t.IdTheLoaiNavigation)
                .OrderByDescending(t => t.NgayDang) // Sắp xếp theo ngày đăng mới nhất
                .AsQueryable();

            if (idTheLoai.HasValue)
            {
                query = query.Where(t => t.IdTheLoai == idTheLoai);
            }

            ViewData["IdTheLoai"] = new SelectList(_context.ChuDes, "IdchuDe", "TenChuDe", idTheLoai);
            return View(await query.ToListAsync());
        }


        // GET: Admin/TinTucs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinTuc = await _context.TinTucs
                .Include(t => t.IdTheLoaiNavigation)
                .FirstOrDefaultAsync(m => m.IdTinTuc == id);
            if (tinTuc == null)
            {
                return NotFound();
            }
            // Lấy danh sách bình luận thuộc tin tức này
            var binhLuans = await _context.BinhLuanBaiViets
                .Where(b => b.IdTinTuc == id)
                .Include(b => b.NguoiDung) // Lấy thông tin người bình luận
                .OrderByDescending(b => b.NgayBinhLuan)
                .ToListAsync();

            ViewBag.BinhLuans = binhLuans;


            return View(tinTuc);
        }

        // GET: Admin/TinTucs/Create
        public IActionResult Create()
        {
            ViewData["IdTheLoai"] = new SelectList(_context.ChuDes, "IdchuDe", "TenChuDe");
            return View();
        }
        //upload file
        public string? Upload(IFormFile file)
        {
            string? uploadFileName = null;
            if (file != null)
            {
                uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = $"wwwroot\\img\\tintuc\\{uploadFileName}";
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return uploadFileName;
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                // Giữ nguyên tên ảnh khi lưu
                var fileName = Path.GetFileName(upload.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/tintuc", fileName);

                // Kiểm tra nếu tệp đã tồn tại thì thêm timestamp để tránh ghi đè
                if (System.IO.File.Exists(filePath))
                {
                    string extension = Path.GetExtension(fileName);
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    fileName = $"{nameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/tintuc", fileName);
                }

                // Lưu ảnh vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                // Trả về đường dẫn ảnh để CKEditor hiển thị
                var url = $"/img/tintuc/{fileName}";
                return Json(new { uploaded = 1, fileName = fileName, url = url });
            }

            return Json(new { uploaded = 0, error = new { message = "Lỗi khi tải ảnh lên" } });
        }


        // POST: Admin/TinTucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile AnhDaiDien, [Bind("IdTinTuc,TieuDe,NoiDung,AnhDaiDien,NgayDang,LuotXem,IdTheLoai")] TinTuc tinTuc)
        {
            if (await _context.TinTucs.AnyAsync(g => g.TieuDe == tinTuc.TieuDe))
            {
                ModelState.AddModelError("TieuDe", "Tiêu Đề tin tức này đã có rồi, vui lòng nhập tên khác.");
            }
            if (ModelState.IsValid)
            {
                tinTuc.AnhDaiDien = Upload(AnhDaiDien);
                _context.Add(tinTuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdTheLoai"] = new SelectList(_context.ChuDes, "IdchuDe", "TenChuDe", tinTuc.IdTheLoai);
            return View(tinTuc);
        }

        // GET: Admin/TinTucs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc == null)
            {
                return NotFound();
            }
            ViewData["IdTheLoai"] = new SelectList(_context.ChuDes, "IdchuDe", "TenChuDe", tinTuc.IdTheLoai);
            return View(tinTuc);
        }

        // POST: Admin/TinTucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile? AnhDaiDien, int id, [Bind("IdTinTuc,TieuDe,NoiDung,AnhDaiDien,NgayDang,LuotXem,IdTheLoai")] TinTuc tinTuc)
        {
            if (id != tinTuc.IdTinTuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tinTucCu = await _context.TinTucs.AsNoTracking().FirstOrDefaultAsync(t => t.IdTinTuc == id);
                    if (tinTucCu == null)
                    {
                        return NotFound();
                    }

                    // Nếu không chọn ảnh mới, giữ nguyên ảnh cũ
                    if (AnhDaiDien != null)
                    {
                        tinTuc.AnhDaiDien = Upload(AnhDaiDien);
                    }
                    else
                    {
                        tinTuc.AnhDaiDien = tinTucCu.AnhDaiDien;
                    }
                    // Giữ nguyên số lượt xem từ cơ sở dữ liệu, nếu không có thì mặc định là 0
                    tinTuc.LuotXem = tinTucCu.LuotXem ?? 0;
                    // Cập nhật ngày đăng thành ngày hiện tại khi chỉnh sửa
                    tinTuc.NgayDang = DateTime.Now;

                    _context.Update(tinTuc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TinTucExists(tinTuc.IdTinTuc))
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
            ViewData["IdTheLoai"] = new SelectList(_context.ChuDes, "IdchuDe", "TenChuDe", tinTuc.IdTheLoai);
            return View(tinTuc);
        }

        // GET: Admin/TinTucs/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdv = await _context.TinTucs.FindAsync(id);
            if (gdv != null)
            {
                _context.TinTucs.Remove(gdv);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private bool TinTucExists(int id)
        {
            return _context.TinTucs.Any(e => e.IdTinTuc == id);
        }
    }
}
