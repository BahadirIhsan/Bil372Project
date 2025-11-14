using Microsoft.AspNetCore.Mvc;
using Bil372Project.PresentationLayer.Models;

namespace Bil372Project.PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: backend ile gerçek kontrol eklenecek

            // Şimdilik login başarılı varsay ve Dashboard'a yönlendir
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: backend ile kullanıcı kaydı eklenecek

            // Kayıt sonrası giriş sayfasına gönder
            return RedirectToAction("Login");
        }
    }
}