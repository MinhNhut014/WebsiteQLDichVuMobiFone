namespace WebsiteQLDichVuMobiFone.Models.VNPay
{
    public class PaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public double Amount { get; set; }

        // Additional properties
        public int IdSim { get; set; }
        public int IdGoiDangKy { get; set; }
        public int IdPhuongThucVc { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaDiemNhan { get; set; }
        public string PhuongThucThanhToan { get; set; }

    }
}
