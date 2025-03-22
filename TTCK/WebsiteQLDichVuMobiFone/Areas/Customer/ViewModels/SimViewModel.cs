using WebsiteQLDichVuMobiFone.Models;
using System.Collections.Generic;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.ViewModels
{
    public class SimViewModel
    {
        public int SimId { get; set; }
        public int GoiCuocId { get; set; }
        public string SoThueBao { get; set; }
        public string LoaiSo { get; set; }
        public string KhuVucHoaMang { get; set; }
        public int PhiHoaMang { get; set; }
        public string TenGoiCuoc { get; set; }
        public int GiaGoiCuoc { get; set; }
        public List<GoiDangKy> GoiCuocList { get; set; }
    }
}
