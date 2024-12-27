using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropPulse.Data;
using PropPulse.Models;

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
                string hashedPassword = HashPassword(model.Password);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null || !VerifyPassword(model.Password, user.Password))
                {
                    ModelState.AddModelError(string.Empty, "Eposta veya şifre yanlış.");
                    return View(model);
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                // Kullanıcı bilgilerini ViewBag'e ekleme
                ViewBag.FirstName = $"{user.FirstName}";
                ViewBag.LastName = $"{user.LastName}";
                ViewBag.UserEmail = user.Email;
                ViewBag.UserPhone = user.PhoneNumber;
                ViewBag.UserDateOfBirth = user.DateOfBirth.ToString("dd MM yyyy");

                // Kullanıcı ID'sini cookie olarak eklemek
                HttpContext.Response.Cookies.Append("UserID", user.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = false
                });

                // Ana sayfaya yönlendirme
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,PhoneNumber,Email,Password,DateOfBirth,ProfilePhoto,CreatedAt")] User user)
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Profile
        public IActionResult Profile()
        {
            // Kullanıcı ID'sini claims üzerinden alıyoruz
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Eğer kullanıcı ID'si yoksa, ziyaretçi olarak varsayıyoruz
                ViewBag.FullName = "Ziyaretçi";
                ViewBag.Email = "ziyaretci@ornek.com";
                ViewBag.PhoneNumber = "N/A";
                ViewBag.DateOfBirth = "N/A";
                return View();
            }

            // Kullanıcı ID'sini int'e dönüştürüyoruz
            if (int.TryParse(userId, out int userIdInt))
            {
                // Kullanıcıyı veritabanından buluyoruz
                var user = _context.User.FirstOrDefault(u => u.Id == userIdInt);

                if (user != null)
                {
                    // Kullanıcı bilgilerini ViewBag'e ekliyoruz
                    ViewBag.FirstName = user.FirstName;
                    ViewBag.LastName = user.LastName;
                    ViewBag.Email = user.Email;
                    ViewBag.PhoneNumber = user.PhoneNumber;
                    ViewBag.DateOfBirth = user.DateOfBirth.ToString("dd MM yyyy");
                }
                else
                {
                    ViewBag.ErrorMessage = "Kullanıcı bulunamadı.";
                }

                // Kullanıcıya ait ilanları alıyoruz
                var models = _context.Properties
                    .Where(p => p.UserID == userIdInt)
                    .ToList();

                // İlanları ViewBag'e ekliyoruz
                ViewBag.UserProperties = models;
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı bilgisi.";
            }

            return View();
        }


        // GET: Users/Favorites
        public IActionResult Favorites()
        {
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
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
        }
    }
}
