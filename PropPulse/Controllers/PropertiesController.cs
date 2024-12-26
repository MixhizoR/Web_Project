using Microsoft.AspNetCore.Mvc;
using PropPulse.Data;
using PropPulse.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropPulse.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly PropPulseContext _context;

        public PropertiesController(PropPulseContext context)
        {
            _context = context;
        }

        // POST: Properties/Create
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

                // Fotoğraf validasyonu: Yalnızca .jpg ve .jpeg formatına izin veriyoruz
                if (Photos != null && Photos.Any())
                {
                    foreach (var photo in Photos)
                    {
                        // Dosyanın uzantısını kontrol et
                        var extension = Path.GetExtension(photo.FileName).ToLower();

                        // Sadece .jpg veya .jpeg dosyalarına izin ver
                        if (extension != ".jpg" && extension != ".jpeg")
                        {
                            ModelState.AddModelError("Photos", "Sadece .jpg veya .jpeg formatında fotoğraf yükleyebilirsiniz.");
                            return View(property);  // Hata varsa, formu tekrar yükle
                        }
                    }
                }

                // Fotoğrafları yükleme ve kaydetme
                if (Photos != null && Photos.Any())
                {
                    property.Photos = new List<string>(); // Fotoğraf listesi başlatılır
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Klasör yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var photo in Photos)
                    {
                        if (photo.Length > 0)
                        {
                            var fileName = Path.GetFileName(photo.FileName);
                            var filePath = Path.Combine(uploadsFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(stream);
                            }

                            // Kaydedilen dosya yolunu ekle
                            property.Photos.Add($"/uploads/{fileName}");
                        }
                    }
                }

                // Kullanıcı ID'sini Property'e Ekle
                property.UserID = userId;

                // Veritabanına kaydet
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Başarılı işlem sonrası yönlendirme
            }
            catch (Exception ex)
            {
                // Genel hata yakalama
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(property);
            }
        }

        // GET: Properties/Favorites
        public IActionResult Favorites()
        {
            // Kullanıcının favorilerini burada sağlayın (örnek veri)
            var favorites = new List<string> { "Favori 1", "Favori 2", "Favori 3" };
            ViewData["Title"] = "Favorilerim";

            // Veriyi View'a gönderiyoruz
            return View(favorites);  // Model geçiyoruz
        }

        // GET: Properties/MyAds
        public IActionResult MyAds()
        {
            // Kullanıcının ilanlarına dair örnek veri
            var ads = new List<string> { "İlan 1", "İlan 2", "İlan 3" };
            ViewData["Title"] = "İlanlarım";

            // View'a model gönderiyoruz
            return View(ads); // Modeli View'a gönderiyoruz
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            // İlan verme sayfası için örnek veriler
            var adTypes = new List<string> { "Kiralık", "Satılık" }; // İlan türlerini oluşturuyoruz
            ViewData["Title"] = "İlan Ver";
            ViewData["AdTypes"] = adTypes;  // İlan türlerini view'a gönderiyoruz

            // View'a model gönderiyoruz
            return View();
        }

        // GET: Properties/CreateAdForm
        public IActionResult CreateAdForm()
        {
            // İlan oluşturma formu (örnek veri gerekirse eklenebilir)
            return View();
        }
    }
}
