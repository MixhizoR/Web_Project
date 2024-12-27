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

                if (ModelState.IsValid)
                {
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
            // İlanı veritabanından bul, Photos'u dahil etmiyoruz çünkü koleksiyon değil
            var property = _context.Properties
                .FirstOrDefault(p => p.Id == id);

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
        public async Task<IActionResult> Edit(int id, Property model, List<IFormFile> newPhotos, List<string> existingPhotosToDelete)
        {
            // İlanı veritabanından bul, Photos'u dahil etmiyoruz çünkü koleksiyon değil
            var property = _context.Properties
                .FirstOrDefault(p => p.Id == id);

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

            // İlanın temel bilgilerini güncelle
            property.Title = model.Title;
            property.Price = model.Price;
            property.Area = model.Area;
            property.Address = model.Address;
            property.Description = model.Description;
            property.IsFurnished = model.IsFurnished;
            property.RoomCount = model.RoomCount;

            // Yeni fotoğrafları kaydet
            if (newPhotos != null && newPhotos.Any())
            {
                foreach (var photo in newPhotos)
                {
                    var extension = Path.GetExtension(photo.FileName).ToLower();

                    // Sadece .jpg ve .jpeg dosyalarına izin ver
                    if (extension != ".jpg" && extension != ".jpeg")
                    {
                        ModelState.AddModelError("Photos", "Sadece .jpg veya .jpeg formatında fotoğraf yükleyebilirsiniz.");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Klasör yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // Yeni fotoğrafı ekle
                    property.Photos.Add($"/uploads/{fileName}");
                }
            }

            // Eski fotoğrafları sil
            if (existingPhotosToDelete != null && existingPhotosToDelete.Any())
            {
                foreach (var photoPath in existingPhotosToDelete)
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoPath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath); // Dosyayı sil
                    }

                    property.Photos.Remove(photoPath); // DB'den kaldır
                }
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

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
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            return View(property); // İlanı detaylar view'ine gönderiyoruz
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
