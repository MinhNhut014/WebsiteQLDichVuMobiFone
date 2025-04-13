namespace WebsiteQLDichVuMobiFone.Models.VNPay
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public int IdSim { get; set; }
        public int IdGoiDangKy { get; set; }
        public int IdPhuongThucVc { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaDiemNhan { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string LoaiDichVu { get; set; }  // <- thêm dòng này
    }
}
