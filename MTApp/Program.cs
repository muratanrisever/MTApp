using Microsoft.EntityFrameworkCore;
using MTApp.Data;
using MTApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // Cookie tabanlý kimlik doðrulama için
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için

var builder = WebApplication.CreateBuilder(args);

// Uygulamanýn servislerini konteynere ekle.

// Veritabaný baðlantý dizesini al.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext'i servislere ekle ve SQL Server kullanmasýný belirt.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Kimlik doðrulama servislerini ekle.
// Cookie tabanlý kimlik doðrulamasýný yapýlandýrýyoruz.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriþ yapýlmadýðýnda yönlendirilecek sayfa
        options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eriþimde yönlendirilecek sayfa
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum süresi
        options.SlidingExpiration = true; // Oturum süresini kaydýr
    });

// Yetkilendirme servislerini ekle.
builder.Services.AddAuthorization(options =>
{
    // Örnek bir rol bazlý politika tanýmlayabiliriz.
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireHRRole", policy => policy.RequireRole("Admin", "HR"));
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Razor sayfalarýný kullanabilmek için (Identity UI olmasa da bazý durumlarda gerekli olabilir)

var app = builder.Build();

// HTTP istek iþlem hattýný yapýlandýr.
if (app.Environment.IsDevelopment())
{
    // Geliþtirme ortamýnda veritabaný hatalarý için özel sayfa göster.
    // Identity UI kaldýrýldýðý için UseMigrationsEndPoint'i kaldýrýyoruz.
    // app.UseMigrationsEndPoint(); // Bu satýrý kaldýrýyoruz
}
else
{
    // Üretim ortamýnda hata durumunda ana sayfaya yönlendir.
    app.UseExceptionHandler("/Home/Error");
    // HSTS (HTTP Strict Transport Security) politikasý uygula.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Kimlik doðrulama middleware'ini UseRouting'den sonra ve UseAuthorization'dan önce ekleyin.
// Bu sýralama çok önemlidir!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor sayfalarýný (Identity sayfalarý gibi) haritala.
// Identity UI kaldýrýldýðý için MapRazorPages'i de kaldýrýyoruz.
// app.MapRazorPages(); // Bu satýrý kaldýrýyoruz, eðer sadece MVC kullanýyorsanýz.
// Eðer Identity UI'ý tamamen kaldýrdýysanýz ve Razor Pages kullanmýyorsanýz bu satýrý kaldýrýn.
// Ancak _LoginPartial gibi bazý yerlerde Razor Pages'e ihtiyaç duyulabilir.
// Eðer _LoginPartial'ý da kendimiz yapýyorsak bu satýr kaldýrýlabilir.

app.Run();