using Microsoft.AspNetCore.Mvc;
using PropPulse.Models;
using System.Diagnostics;

namespace PropPulse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Kay�t i�lemleri burada yap�labilir
                // Kullan�c�y� veritaban�na kaydedebilirsiniz
                return RedirectToAction("Index"); // Ba�ka bir sayfaya y�nlendirme
            }

            return View(user); // Modeli yeniden d�nd�r�r
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
