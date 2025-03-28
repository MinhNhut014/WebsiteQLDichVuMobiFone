using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebsiteQLDichVuMobiFone.Filters
{
    [Area("Admin")]
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var user = session.GetString("nguoidung"); // Kiểm tra session đăng nhập

            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectToActionResult("Index", "DangNhap", new { area = "" });
            }

            base.OnActionExecuting(context);
        }
    }
}
