using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.ViewModels
{
    public class SimDangKyViewModel
    {
        public int SimId { get; set; }
        public int GoiCuocId { get; set; }
        public string SoThueBao { get; set; }
        public string LoaiSo { get; set; }
        public string TenGoiCuoc { get; set; }
        public decimal GiaGoiCuoc { get; set; }
        public decimal PhiHoaMang { get; set; }
        public decimal GiaVanChuyen { get; set; }
        public List<PhuongThucVanChuyen> PhuongThucVanChuyens { get; set; }
    }
}
