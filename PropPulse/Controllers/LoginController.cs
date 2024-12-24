using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

namespace YourNamespace.Controllers
{
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Mock validation. Replace with actual user authentication logic.
                if (model.Email == "test@example.com" && model.Password == "password")
                {
                    // Add logic to set authentication cookie/session here.
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            // Add logic to clear authentication cookie/session here.
            return RedirectToAction("Index", "Home");
        }
    }
}
