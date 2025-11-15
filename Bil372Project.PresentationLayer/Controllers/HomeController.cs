using System.Security.Claims;
using Bil372Project.BusinessLayer;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Services;
using Bil372Project.PresentationLayer.Controllers.Helper;
using Bil372Project.PresentationLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bil372Project.PresentationLayer.Controllers;

public class HomeController : Controller
{
    private readonly IUserMeasurementService _userMeasurementService;
    private readonly IAppUserService _userService;

    public HomeController(IUserMeasurementService userMeasurementService, IAppUserService userService)
    {
        _userMeasurementService = userMeasurementService;
        _userService = userService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var stats = await _userMeasurementService.GetDashboardStatsAsync(userId);

        var model = new DashboardViewModel
        {
            CurrentWeightKg          = stats.CurrentWeightKg,
            CurrentBmi               = stats.CurrentBmi,
            WeightChangeLastMonthKg  = stats.WeightChangeLastMonthKg,
            BmiCategory              = stats.BmiCategory
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Values()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dto = await _userMeasurementService.GetMeasureAsync(userId);

        var model = new MeasurementViewModel();

        if (dto != null)
        {
            model.Gender        = dto.Gender;
            model.Age           = dto.Age;
            model.HeightCm      = dto.HeightCm;
            model.WeightKg      = dto.WeightKg;
            model.Allergies     = dto.Allergies;
            model.Diseases      = dto.Diseases;
            model.DislikedFoods = dto.DislikedFoods;
            model.LastUpdatedAt = dto.LastUpdatedAt;
            model.LastUpdatedText = TimeHelper.GetRelativeTimeText(dto.LastUpdatedAt);
        }
        else
        {
            model.LastUpdatedText = "Henüz hiç güncellenmedi";
        }

        return View(model);
    }

    // POST: Değerlerim
    [HttpPost]
    public async Task<IActionResult> Values(MeasurementViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var input = new UserMeasureInput
        {
            Gender        = model.Gender,
            Age           = model.Age,
            HeightCm      = model.HeightCm,
            WeightKg      = model.WeightKg,
            Allergies     = model.Allergies,
            Diseases      = model.Diseases,
            DislikedFoods = model.DislikedFoods
        };

        await _userMeasurementService.SaveMeasureAsync(userId, input);

        TempData["ValuesUpdated"] = "Ölçümleriniz güncellendi.";
        
        return RedirectToAction("Values");
    }

    public IActionResult History()
    {
        return View();
    }

    public IActionResult NewProgram()
    {
        return View();
    }

    [HttpGet]
        public async Task<IActionResult> Settings()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var settings = await _userService.GetSettingsAsync(userId);
            if (settings == null) return NotFound();

            var model = new UserSettingsViewModel
            {
                FullName    = settings.FullName,
                Email       = settings.Email,
                PhoneNumber = settings.PhoneNumber,
                BirthDate   = settings.BirthDate,
                City        = settings.City,
                Country     = settings.Country,
                Bio         = settings.Bio
            };

            ViewBag.PasswordModel = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserSettingsViewModel model)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!ModelState.IsValid)
            {
                ViewBag.PasswordModel = new ChangePasswordViewModel();
                return View("Settings", model);
            }

            var dto = new UserSettingsDto
            {
                FullName    = model.FullName,
                Email       = model.Email,
                PhoneNumber = model.PhoneNumber,
                BirthDate   = model.BirthDate,
                City        = model.City,
                Country     = model.Country,
                Bio         = model.Bio
            };

            var result = await _userService.UpdateSettingsAsync(userId, dto);

            if (!result.Succeeded)
            {
                AddErrorsToModelState(result);
                ViewBag.PasswordModel = new ChangePasswordViewModel();
                return View("Settings", model);
            }

            TempData["ProfileUpdated"] = "Profil bilgileriniz güncellendi.";
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Profil bilgilerini (Settings sayfası için) her durumda çekeceğiz
            var settings = await _userService.GetSettingsAsync(userId);
            var profileModel = new UserSettingsViewModel
            {
                FullName    = settings?.FullName ?? "",
                Email       = settings?.Email ?? "",
                PhoneNumber = settings?.PhoneNumber,
                BirthDate   = settings?.BirthDate,
                City        = settings?.City,
                Country     = settings?.Country,
                Bio         = settings?.Bio
            };

            if (!ModelState.IsValid)
            {
                ViewBag.PasswordModel = model;
                return View("Settings", profileModel);
            }

            var dto = new ChangePasswordDto
            {
                UserId             = userId,
                CurrentPassword    = model.CurrentPassword,
                NewPassword        = model.NewPassword,
                ConfirmNewPassword = model.ConfirmNewPassword
            };

            var result = await _userService.ChangePasswordAsync(dto);

            if (!result.Succeeded)
            {
                AddErrorsToModelState(result);
                ViewBag.PasswordModel = model;
                return View("Settings", profileModel);
            }

            TempData["PasswordUpdated"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("Settings");
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