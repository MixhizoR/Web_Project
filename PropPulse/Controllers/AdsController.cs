using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace PropPulse.Controllers
{
    public class AdsController : Controller
    {
        // GET: Ads/Favorites
        public IActionResult Favorites()
        {
            // Kullanıcının favorilerini burada sağlayın (örnek veri)
            var favorites = new List<string> { "Favori 1", "Favori 2", "Favori 3" };
            ViewData["Title"] = "Favorilerim";

            // Veriyi View'a gönderiyoruz
            return View(favorites);  // Model geçiyoruz
        }

        // POST: Ads/CreateAd
        [HttpPost]
        public IActionResult CreateAd(string AdTitle, string AdDescription, decimal AdPrice, string AdType)
        {
            // İlan oluşturma işlemleri (örneğin, veritabanına kaydetme)
            TempData["Message"] = "İlan başarıyla oluşturuldu.";
            return RedirectToAction("MyAds");
        }

        // GET: Ads/MyAds
        public IActionResult MyAds()
        {
            // Kullanıcının ilanlarına dair örnek veri
            var ads = new List<string> { "İlan 1", "İlan 2", "İlan 3" };
            ViewData["Title"] = "İlanlarım";

            // View'a model gönderiyoruz
            return View(ads); // Modeli View'a gönderiyoruz
        }

        // GET: Ads/Create
        public IActionResult Create()
        {
            // İlan verme sayfası için örnek veriler
            var adTypes = new List<string> { "Kiralık", "Satılık" }; // İlan türlerini oluşturuyoruz
            ViewData["Title"] = "İlan Ver";
            ViewData["AdTypes"] = adTypes;  // İlan türlerini view'a gönderiyoruz

            // View'a model gönderiyoruz
            return View();
        }

        // GET: Ads/CreateAdForm
        public IActionResult CreateAdForm()
        {
            // İlan oluşturma formu (örnek veri gerekirse eklenebilir)
            return View();
        }
    }
}
