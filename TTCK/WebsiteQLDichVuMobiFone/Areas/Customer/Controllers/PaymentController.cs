using Microsoft.AspNetCore.Mvc;
using WebsiteQLDichVuMobiFone.Models.VNPay;
using WebsiteQLDichVuMobiFone.Services.VNPay;

namespace WebsiteQLDichVuMobiFone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            // Store customer information in session
            HttpContext.Session.SetString("TenKhachHang", model.TenKhachHang);
            HttpContext.Session.SetString("SoDienThoai", model.SoDienThoai);
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("DiaDiemNhan", model.DiaDiemNhan);
            HttpContext.Session.SetString("PhuongThucThanhToan", model.PhuongThucThanhToan);
            HttpContext.Session.SetInt32("IdSim", model.IdSim);
            HttpContext.Session.SetInt32("IdGoiDangKy", model.IdGoiDangKy);
            HttpContext.Session.SetInt32("IdPhuongThucVc", model.IdPhuongThucVc);


            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }

    }
}
