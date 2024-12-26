using Microsoft.AspNetCore.Mvc;
using PropPulse.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using PropPulse.Data;
namespace PropPulse.Controllers
{
    public class AccountController : Controller
    {
        private readonly PropPulseContext _context;

        public AccountController(PropPulseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // E-postayı veritabanında sorgulama
                var user = await _context.User
                                         .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    // Şifreyi kontrol etme
                    if (VerifyPassword(model.Password, user.Password))
                    {
                        // Giriş başarılı
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Şifre hatalı
                        ModelState.AddModelError(string.Empty, "Geçersiz şifre.");
                    }
                }
                else
                {
                    // E-posta bulunamadı
                    ModelState.AddModelError(string.Empty, "E-posta adresi bulunamadı.");
                }
            }

            // Hatalı giriş durumunda, Login sayfasına geri dön
            return View(model);
        }

        // Şifreyi doğrulamak için bir metod
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Bcrypt hash doğrulaması
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
        }
    }
}
