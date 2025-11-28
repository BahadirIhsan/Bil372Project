using System.Security.Claims;
using Bil372Project.BusinessLayer;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Services;
using Bil372Project.PresentationLayer.Controllers.Helper;
using Bil372Project.PresentationLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bil372Project.PresentationLayer.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUserMeasurementService _userMeasurementService;
    private readonly IAppUserService _userService;
    private readonly IDietPlanService _dietPlanService;

    public HomeController(
        IUserMeasurementService userMeasurementService,
        IAppUserService userService,
        IDietPlanService dietPlanService)
    {
        _userMeasurementService = userMeasurementService;
        _userService = userService;
        _dietPlanService = dietPlanService;
    }

    // Dashboard
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var stats = await _userMeasurementService.GetDashboardStatsAsync(userId);

        var model = new DashboardViewModel
        {
            CurrentWeightKg         = stats.CurrentWeightKg,
            CurrentBmi              = stats.CurrentBmi,          // şimdilik null olabilir
            WeightChangeLastMonthKg = stats.WeightChangeLastMonthKg,
            BmiCategory             = stats.BmiCategory
        };

        return View(model);
    }

    // Değerlerim (son ölçüm)
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
            model.Diseases      = dto.Diseases;
            model.ActivityLevel = dto.ActivityLevel;
            model.DietaryPreference = dto.DietaryPreference;
            model.LastUpdatedAt = dto.LastUpdatedAt;
            model.LastUpdatedText = TimeHelper.GetRelativeTimeText(dto.LastUpdatedAt);
        }
        else
        {
            model.LastUpdatedText = "Henüz hiç güncellenmedi";
        }

        return View(model);
    }

    // POST: Değerlerim – sadece ölçü kaydı, diyet üretmiyor
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Values(MeasurementViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var input = new UserMeasureInput
        {
            Gender            = model.Gender,
            Age               = model.Age,
            HeightCm          = model.HeightCm,
            WeightKg          = model.WeightKg,
            Diseases          = model.Diseases,
            ActivityLevel     = model.ActivityLevel,
            DietaryPreference = model.DietaryPreference
        };

        await _userMeasurementService.SaveMeasureAsync(userId, input);

        TempData["ValuesUpdated"] = "Ölçümleriniz güncellendi.";
        
        return RedirectToAction("Values");
    }

    // Yeni Program – GET: formu son ölçümle doldur
    [HttpGet]
    public async Task<IActionResult> NewProgram()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dto = await _userMeasurementService.GetMeasureAsync(userId);
        var model = new MeasurementViewModel();

        if (dto != null)
        {
            model.Gender            = dto.Gender;
            model.Age               = dto.Age;
            model.HeightCm          = dto.HeightCm;
            model.WeightKg          = dto.WeightKg;
            model.Diseases          = dto.Diseases;
            model.ActivityLevel     = dto.ActivityLevel;
            model.DietaryPreference = dto.DietaryPreference;
            model.LastUpdatedAt     = dto.LastUpdatedAt;
        }

        return View(model);
    }

    // Yeni Program – POST: ölçüm kaydet + diyet oluştur
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewProgram(MeasurementViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var input = new UserMeasureInput
        {
            Gender            = model.Gender,
            Age               = model.Age,
            HeightCm          = model.HeightCm,
            WeightKg          = model.WeightKg,
            Diseases          = model.Diseases,
            ActivityLevel     = model.ActivityLevel,
            DietaryPreference = model.DietaryPreference
        };

        // 1) Ölçümü kaydet ve ölçüm Id'sini al
        int userMeasureId = await _userMeasurementService.SaveMeasureAsync(userId, input);

        // 2) Bu ölçüme göre diyet oluştur
        await _dietPlanService.CreateDietPlanAsync(userMeasureId);

        TempData["ProgramCreated"] = "Yeni programınız oluşturuldu.";

        // İster History'e, ister özel bir “Son Program” sayfasına atabiliriz
        return RedirectToAction("History");
    }

    // Geçmiş programlar
    [HttpGet]
    public async Task<IActionResult> History()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var plans = await _dietPlanService.GetUserPlansAsync(userId);

        var model = new DietHistoryViewModel
        {
            Plans = plans.Select(p => new DietPlanViewModel
            {
                Id         = p.Id,
                GeneratedAt = p.GeneratedAt,
                Breakfast  = p.Breakfast,
                Lunch      = p.Lunch,
                Dinner     = p.Dinner,
                Snack      = p.Snack
            }).ToList()
        };

        return View(model);
    }

    // Ayarlar / profil (senin mevcut kodun, ufak düzenlemeden başka değişmedim)
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
    [ValidateAntiForgeryToken]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
