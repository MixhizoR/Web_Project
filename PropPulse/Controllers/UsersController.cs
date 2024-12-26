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

                HttpContext.Response.Cookies.Append("UserID", user.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = false
                });

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
            string? userEmail = User.Identity?.Name;

            User user;

            if (!string.IsNullOrEmpty(userEmail))
            {
                user = _context.User.FirstOrDefault(u => u.Email == userEmail);
            }
            else
            {
                user = new User
                {
                    FirstName = "Ziyaretçi",
                    LastName = "",
                    Email = "ziyaretci@ornek.com"
                };
            }

            ViewBag.FullName = $"{user.FirstName} {user.LastName}";
            ViewBag.Email = user.Email;

            return View();
        }

        [HttpPost]
        public IActionResult Update(string FullName, string Email, string Password, IFormFile ProfileImage)
        {
            TempData["Message"] = "Profil bilgileri başarıyla güncellendi.";
            return RedirectToAction("Profile");
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
