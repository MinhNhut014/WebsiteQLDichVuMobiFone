using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Models.ViewModels
{
    public class DichVuDiDongViewModel
    {
        public IEnumerable<LoaiDichVuDiDong> DanhMucSanPham { get; set; }
        public IEnumerable<GoiDangKy> GoiDangKy { get; set; }
        public IEnumerable<GoiDangKy> GoiCuocNoiBat { get; set; }
    }
}
