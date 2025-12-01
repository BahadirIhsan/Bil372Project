using System.Security.Claims;
using Bil372Project.BusinessLayer;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Services;
using Bil372Project.PresentationLayer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bil372Project.PresentationLayer.Controllers;

public class AccountController : Controller
{
        private readonly IAppUserService _userService;

        public AccountController(IAppUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.LoginAsync(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
                return View(model);
            }

            // Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(14)
                    : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
            
            if (user.IsAdmin)
                return RedirectToAction(nameof(RoleChoice));

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult RoleChoice()
        {
            var isAdmin = User.HasClaim("IsAdmin", bool.TrueString) || User.IsInRole("Admin");

            if (!isAdmin)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Aynı email var mı kontrol
            var dto = new RegisterUserDto
            {
                FullName        = model.FullName,
                Email           = model.Email,
                Password        = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _userService.RegisterAsync(dto);

            if (!result.Succeeded)
            {
                AddErrorsToModelState(result);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private void AddErrorsToModelState(ServiceResult result)
        {
            foreach (var error in result.Errors)
            {
                var key = string.IsNullOrWhiteSpace(error.Key)
                    ? string.Empty
                    : error.Key;

                foreach (var message in error.Value)
                {
                    ModelState.AddModelError(key, message);
                }
            }
        }
    }
