using WebsiteQLDichVuMobiFone.Models.VNPay;

namespace WebsiteQLDichVuMobiFone.Services.VNPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
