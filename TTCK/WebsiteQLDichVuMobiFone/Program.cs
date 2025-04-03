using WebsiteQLDichVuMobiFone.Data;
using Microsoft.AspNetCore.Identity;
using WebsiteQLDichVuMobiFone.Models;
using WebsiteQLDichVuMobiFone.Services.VNPay;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IPasswordHasher<NguoiDung>, PasswordHasher<NguoiDung>>();
//connect VnPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();
// Thêm dịch vụ Session vào container
builder.Services.AddDistributedMemoryCache();  // Đảm bảo có bộ nhớ đệm
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;  // Cookie chỉ được truy cập qua HTTP
    options.Cookie.IsEssential = true;  // Đảm bảo cookie không bị block
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/tintuc");
if (!Directory.Exists(path))
{
    Directory.CreateDirectory(path);
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseSession(); // Đặt ngay sau UseRouting
app.UseAuthentication(); // Thêm nếu sử dụng Identity

app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
    endpoints.MapControllerRoute(
        name: "GetLoaiGoiByDichVu",
        pattern: "Admin/GoiDangKies/GetLoaiGoiByDichVu");
});
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
