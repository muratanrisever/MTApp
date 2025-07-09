using Microsoft.AspNetCore.Mvc;
using MTApp.Data;
using MTApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore; // Şifre hashleme için

namespace MTApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        // Kayıt formunu gösterir.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        // Yeni kullanıcı kaydını işler.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının veya e-postanın zaten var olup olmadığını kontrol et.
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten mevcut.");
                    return View(model);
                }
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                    return View(model);
                }

                // Şifreyi hash'le (güvenlik için çok önemli!)
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash); // Şifreyi PasswordHash alanına alıyoruz, modelde Password diye bir alan yok.

                // Rolü varsayılan olarak "User" olarak ayarla
                model.Role = "User";
                model.RegistrationDate = DateTime.Now;

                _context.Add(model);
                await _context.SaveChangesAsync();

                // Kayıt başarılı olduktan sonra otomatik giriş yapabilir veya giriş sayfasına yönlendirebiliriz.
                // Şimdilik giriş sayfasına yönlendirelim.
                TempData["SuccessMessage"] = "Kaydınız başarıyla oluşturuldu. Lütfen giriş yapın.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: /Account/Login
        // Giriş formunu gösterir.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // Kullanıcı girişini işler.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View();
            }

            // Kimlik doğrulama başarılı olursa ClaimsIdentity oluştur.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Kullanıcının rolünü ekle
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // IsPersistent = true, // Beni hatırla özelliği için
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Oturum süresi
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _context.SaveChanges(); // Oturum bilgilerini kaydet (opsiyonel, genelde gerekmez)

            // Eğer bir returnUrl varsa oraya yönlendir, yoksa ana sayfaya.
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        // Kullanıcı çıkışını işler.
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}