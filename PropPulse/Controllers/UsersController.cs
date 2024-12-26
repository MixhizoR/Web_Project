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
                // Find the user in the database by email
                var user = _context.User.FirstOrDefault(u => u.Email == model.Email);

                // If the user exists and the password is correct
                if (user != null && VerifyPassword(model.Password, user.Password))
                {
                    // Create claims for the user
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),  // Can also store user ID or email here
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                    // Create claims identity
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Create authentication properties
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true  // Keep the user logged in after browser is closed
                    };

                    // Sign in the user by adding a cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");  // Redirect to a protected page
                }

                // If credentials are invalid, show error message
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);  // Return to login page with validation errors
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

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                // Profil fotoğrafı güncelleme işlemi
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", ProfileImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(stream);
                }
            }

            TempData["Message"] = "Profil bilgileri başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }

        // GET: Users/MyAds
        public IActionResult MyAds()
        {
            // Kullanıcının ilanlarını burada sağlayın (örnek veri)
            var ads = new List<string> { "İlan 1", "İlan 2", "İlan 3" };
            ViewData["Title"] = "İlanlarım";
            return View(ads);
        }

        // POST: Users/CreateAd
        [HttpPost]
        public IActionResult CreateAd(string AdTitle, string AdDescription, decimal AdPrice)
        {
            // İlan oluşturma işlemleri (örneğin, veritabanına kaydetme)
            TempData["Message"] = "İlan başarıyla oluşturuldu.";
            return RedirectToAction("MyAds");
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
