using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using PropPulse.Data;
using PropPulse.Models;
using Microsoft.AspNetCore.Authorization;

namespace PropPulse.Controllers
{
    public class UsersController : Controller
    {
        private readonly PropPulseContext _context;

        public UsersController(PropPulseContext context)
        {
            _context = context;
        }


        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Şifreyi Bcrypt ile hash'le
                string hashedPassword = HashPassword(model.Password);

                // Yeni kullanıcı oluştur
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = hashedPassword,
                    DateOfBirth = model.DateOfBirth,
                    CreatedAt = DateTime.UtcNow
                };

                // Kullanıcıyı veritabanına kaydet
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login", "Users");
            }

            return View("Create", model);

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı veritabanında email ile bul
                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == model.Email);

                // Kullanıcı bulunamadıysa hata mesajı ekle
                if (user == null || !VerifyPassword(model.Password, user.Password))
                {
                    ModelState.AddModelError(string.Empty, "Eposta veya sifre yanlış.");
                    return View(model);  // Hata mesajı ile birlikte giriş sayfasına dön
                }

                // Kullanıcı doğrulandıysa, kullanıcı için claim'ler oluştur
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                // Claim kimliğini oluştur
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Kimlik doğrulama özelliklerini oluştur
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false  // Tarayıcı kapatıldığında kullanıcıyı oturumda tut
                };

                // Kullanıcıyı oturum açtır
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                HttpContext.Response.Cookies.Append("UserID", user.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30), // Çerezin geçerlilik süresi
                    HttpOnly = true, // JavaScript erişimini engeller
                    Secure = false // Sadece HTTPS üzerinden gönderilmesini sağlar
                });

                return RedirectToAction("Index", "Home");  // Korunan bir sayfaya yönlendir
            }

            return View(model);  // Geçersiz model durumunda giriş sayfasına dön
        }




        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,PhoneNumber,Email,Password,DateOfBirth,ProfilePhoto,CreatedAt,")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Profile
        [Authorize] // Sadece giriş yapmış kullanıcılar bu sayfaya erişebilir
        public IActionResult Profile()
        {
            // Oturum açmış kullanıcının kimliğini al
            string? userEmail = User.Identity.Name;

            // Kullanıcı bilgilerini veritabanından getir
            var user = _context.User.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            // ViewBag ile bilgileri gönder
            ViewBag.FullName = $"{user.FirstName} {user.LastName}";
            ViewBag.Email = user.Email;

            return View();
        }


        // POST: Users/Update
        [HttpPost]
        public IActionResult Update(string FullName, string Email, string Password, IFormFile ProfileImage)
        {
            // Profil bilgilerini güncelleme işlemi yapılabilir (örneğin, veritabanına kaydetme)
            // Burada sadece basit bir mesaj dönüyoruz.

            TempData["Message"] = "Profil bilgileri başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }

        // GET: Users/Favorites
        public IActionResult Favorites()
        {
            // Kullanıcının favorilerini burada sağlayın (örnek veri)
            var favorites = new List<string> { "Favori 1", "Favori 2" };
            ViewData["Title"] = "Favorilerim";
            return View(favorites);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private string HashPassword(string password)
        {
            // Bcrypt hashleme
            return BCrypt.Net.BCrypt.HashPassword(password); // return HashPassword(password);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Assuming you have hashed the passwords using Bcrypt while registering the user
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
        }

    }
}
