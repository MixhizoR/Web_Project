using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropPulse.Data;
using PropPulse.Models;
using System.Collections.Generic;

namespace PropPulse.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly PropPulseContext _context;
        public PropertiesController(PropPulseContext context)
        {
            _context = context;
        }
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, List<IFormFile> Photos)
        {
            try
            {
                // Cookie'den Kullanıcı ID'sini Al
                if (!HttpContext.Request.Cookies.TryGetValue("UserID", out var userIdValue) || !int.TryParse(userIdValue, out var userId))
                {
                    // Eğer cookie yoksa veya geçersizse, hata döndür.
                    ModelState.AddModelError(string.Empty, "Giriş yapmanız gerekiyor.");
                    return View(property);
                }

                //Fotoğraf validasyonu
                //if (Photos.Count < 3 || Photos.Count > 5)
                //{
                //    ModelState.AddModelError("Photos", "En az 3, en fazla 5 fotoğraf yüklemelisiniz.");
                //}

                if (ModelState.IsValid)
                {
                    // Fotoğrafları yükleme ve kaydetme
                    //property.Photos = new List<string>();
                    //foreach (var photo in Photos)
                    //{
                    //    if (photo.Length > 0)
                    //    {
                    //        try
                    //        {
                    //            var fileName = Path.GetFileName(photo.FileName);
                    //            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    //            var filePath = Path.Combine(uploadsFolder, fileName);

                    //            // Klasör yoksa oluştur
                    //            if (!Directory.Exists(uploadsFolder))
                    //            {
                    //                Directory.CreateDirectory(uploadsFolder);
                    //            }

                    //            // Dosyayı kaydet
                    //            using (var stream = new FileStream(filePath, FileMode.Create))
                    //            {
                    //                await photo.CopyToAsync(stream);
                    //            }

                    //            // Kaydedilen dosya yolunu ekle
                    //            property.Photos.Add($"/uploads/{fileName}");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            // Fotoğraf yükleme hatası loglama
                    //            Console.WriteLine($"Fotoğraf yüklenirken hata oluştu: {ex.Message}");
                    //            ModelState.AddModelError("Photos", "Bazı fotoğraflar yüklenemedi. Lütfen tekrar deneyin.");
                    //            return View(property);
                    //        }
                    //    }
                    //}

                   // Kullanıcı ID'sini Property'e Ekle
                property.UserID = userId;

                    // Veritabanına kaydet
                    _context.Properties.Add(property);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home"); // Başarılı işlem sonrası yönlendirme
                }

                return View(property); // Validasyon hatası varsa formu yeniden yükle
            }
            catch (Exception ex)
            {
                // Genel hata yakalama
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(property);
            }
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
