using Microsoft.AspNetCore.Authorization; // Authorize özniteliği için
using Microsoft.AspNetCore.Mvc;
using MTApp.Models;
using System.Diagnostics;

namespace MTApp.Controllers
{
    // HomeController'daki tüm aksiyonlar için kimlik doğrulama gerektirir.
    // Eğer sadece Index aksiyonu için yetkilendirme isterseniz, [Authorize] özniteliğini sadece Index metodunun üzerine ekleyebilirsiniz.
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Ana sayfa (Dashboard) aksiyonu.
        // [Authorize] // Eğer sadece bu aksiyon için yetkilendirme isterseniz buraya ekleyin.
        public IActionResult Index()
        {
            return View();
        }

        // Gizlilik politikası sayfası (örnek).
        // Eğer bu sayfa herkes tarafından erişilebilir olmalıysa, HomeController'a [Authorize] ekledikten sonra
        // bu metodun üzerine [AllowAnonymous] özniteliğini ekleyebilirsiniz.
        public IActionResult Privacy()
        {
            return View();
        }

        // Hata sayfası aksiyonu.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}