using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropPulse.Data;
using PropPulse.Models;
using System.Collections.Generic;
using System.Security.Claims;

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
        public IActionResult MyProperties()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                // Kullanıcı kimliği alınamadıysa hata veya yönlendirme yapılabilir
                return RedirectToAction("Login", "Account");
            }

            var models = _context.Properties
            .Where(p => p.UserID == int.Parse(userId))
            .ToList();

            ViewData["Title"] = "İlanlarım";

            // View'a model gönderiyoruz
            return View(models); // Modeli View'a gönderiyoruz
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

        // GET: Properties/Edit/1
        public IActionResult Edit(int id)
        {
            // İlanı veritabanından bul
            var property = _context.Properties.FirstOrDefault(p => p.Id == id);

            // İlan bulunamazsa hata sayfası göster
            if (property == null)
            {
                return NotFound();
            }

            // İlanı düzenleme formu için View'a gönder
            return View(property);
        }

        // POST: Properties/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Property model)
        {
            // İlanı veritabanından bul
            var property = _context.Properties.FirstOrDefault(p => p.Id == id);

            // İlan bulunamazsa hata sayfası göster
            if (property == null)
            {
                return NotFound();
            }

            // Model geçerli değilse tekrar formu göster
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // İlanın özelliklerini güncelle
            property.Title = model.Title;
            property.Price = model.Price;
            property.Area = model.Area;
            property.Address = model.Address;
            property.Description = model.Description;
            property.IsFurnished = model.IsFurnished;
            property.SquareMeter = model.SquareMeter;

            // Değişiklikleri kaydet
            _context.SaveChanges();

            // Kullanıcıyı ilanlar sayfasına yönlendir
            return RedirectToAction(nameof(MyProperties));
        }


        // GET: Properties/Delete/1
        public IActionResult Delete(int id)
        {
            // İlanı veritabanından bul
            var property = _context.Properties.FirstOrDefault(p => p.Id == id);

            // İlan bulunamazsa hata sayfası göster
            if (property == null)
            {
                return NotFound();
            }

            // İlan bilgilerini View'a gönder
            return View(property);
        }


        // POST: Properties/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Silinecek ilanı veritabanından bul
            var property = _context.Properties.FirstOrDefault(p => p.Id == id);

            // İlan bulunamazsa hata sayfası göster
            if (property == null)
            {
                return NotFound();
            }

            // İlanı veritabanından sil
            _context.Properties.Remove(property);
            _context.SaveChanges();  // Değişiklikleri kaydet

            // Kullanıcıyı ilanlar sayfasına yönlendir
            return RedirectToAction(nameof(MyProperties));
        }

    }
}
