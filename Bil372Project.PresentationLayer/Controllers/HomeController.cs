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
    private static readonly string[] DailyTips =
    {
        "Su tüketimini takip et! Her saat başı bir bardak su içmek enerji seviyeni korumana yardımcı olur.",
        "Güne dengeli bir kahvaltıyla başla; protein ve lif uzun süre tok kalmanı sağlar.",
        "Kısa yürüyüş molaları vererek odaklanmanı ve kalori yakımını artırabilirsin.",
        "Uyku düzenine dikkat et; 7-8 saatlik kaliteli uyku metabolizmanı destekler.",
        "Ara öğünlerde taze meyve veya çiğ kuruyemiş tercih ederek kan şekerini dengede tut.",
        "Ekran başında uzun süre kalıyorsan her 30 dakikada bir esneme hareketleri yap.",
        "Şekerli içecekler yerine limonlu su veya bitki çayı tercih ederek kalori alımını azalt.",
        "Protein hedefini tutturmak için her öğüne yumurta, yoğurt veya baklagil eklemeyi unutma.",
        "Hedeflerini yazılı tut ve haftalık küçük kazanımları kutla; motivasyonun artsın.",
        "Günlük adım sayını takip et; 8.000 adım enerjini yükseltir ve stresini azaltır."
    };
    
    private readonly IUserMeasurementService _userMeasurementService;
    private readonly IAppUserService _userService;
    private readonly IDietPlanService _dietPlanService;
    private readonly IGoalService _goalService;

    public HomeController(
        IUserMeasurementService userMeasurementService,
        IAppUserService userService,
        IDietPlanService dietPlanService,
        IGoalService goalService)
    {
        _userMeasurementService = userMeasurementService;
        _userService = userService;
        _dietPlanService = dietPlanService;
        _goalService = goalService;
    }

    // Dashboard
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var stats = await _userMeasurementService.GetDashboardStatsAsync(userId);
        
        var goal = await _goalService.GetLatestGoalAsync(userId);
        
        var plans = await _dietPlanService.GetUserPlansAsync(userId);
        var latestPlan = plans.FirstOrDefault();
        var weeklyPlanCount = plans.Count(p => p.GeneratedAt >= DateTime.UtcNow.AddDays(-7));


        var model = new DashboardViewModel
        {
            CurrentWeightKg         = stats.CurrentWeightKg,
            CurrentBmi              = stats.CurrentBmi,          // şimdilik null olabilir
            WeightChangeLastMonthKg = stats.WeightChangeLastMonthKg,
            BmiCategory             = stats.BmiCategory,
            TargetWeightKg          = goal?.TargetWeightKg,
            GoalDurationWeeks       = goal?.GoalDurationWeeks,
            DailyWaterTarget        = goal?.DailyWaterTarget,
            WaterConsumedToday      = goal?.WaterConsumedToday,
            WeeklyActivityTarget    = goal?.WeeklyActivityTarget,
            MotivationNote          = goal?.MotivationNote,
            GoalUpdatedAt           = goal?.UpdatedAt,
            WeeklyDietPlans         = weeklyPlanCount,
            TodayPlan = latestPlan == null
                ? null
                : new TodayDietPlanViewModel
                {
                    Breakfast   = latestPlan.Breakfast,
                    Lunch       = latestPlan.Lunch,
                    Dinner      = latestPlan.Dinner,
                    Snack       = latestPlan.Snack,
                    GeneratedAt = latestPlan.GeneratedAt
                },
            TipOfDay = DailyTips[Math.Abs(HashCode.Combine(userId, DateTime.UtcNow.Ticks)) % DailyTips.Length]
        };

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Goals()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await _goalService.GetLatestGoalAsync(userId);

        var model = new GoalViewModel
        {
            TargetWeightKg = goal?.TargetWeightKg,
            GoalDurationWeeks = goal?.GoalDurationWeeks,
            DailyWaterTarget = goal?.DailyWaterTarget,
            WaterConsumedToday = goal?.WaterConsumedToday,
            WeeklyActivityTarget = goal?.WeeklyActivityTarget,
            MotivationNote = goal?.MotivationNote,
            UpdatedAt = goal?.UpdatedAt
        };

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IncrementWater()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var updatedWater = await _goalService.IncrementWaterAsync(userId);

        if (updatedWater == null)
            return BadRequest(new { message = "Önce bir su hedefi eklemelisin." });

        return Json(new { waterConsumedToday = updatedWater });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Goals(GoalViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var input = new UserGoalInput
        {
            TargetWeightKg = model.TargetWeightKg,
            GoalDurationWeeks = model.GoalDurationWeeks,
            DailyWaterTarget = model.DailyWaterTarget,
            WaterConsumedToday = model.WaterConsumedToday,
            WeeklyActivityTarget = model.WeeklyActivityTarget,
            MotivationNote = model.MotivationNote
        };

        await _goalService.SaveGoalAsync(userId, input);
        TempData["GoalUpdated"] = "Hedeflerin kaydedildi.";
        return RedirectToAction("Goals");
    }

    // Değerlerim (son ölçüm)
    [HttpGet]
    public async Task<IActionResult> Values()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dto = await _userMeasurementService.GetMeasureAsync(userId);

        var model = new MeasurementViewModel();

        if (!string.IsNullOrWhiteSpace(dto?.Diseases))
        {
            model.SelectedDiseases = dto!.Diseases
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }
        else
        {
            model.SelectedDiseases = new List<string>();
        }

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

        var diseasesCsv = BuildDiseasesCsv(model.SelectedDiseases);

        var input = new UserMeasureInput
        {
            Gender            = model.Gender,
            Age               = model.Age,
            HeightCm          = model.HeightCm,
            WeightKg          = model.WeightKg,
            Diseases          = diseasesCsv,
            ActivityLevel     = model.ActivityLevel,
            DietaryPreference = model.DietaryPreference
        };

        await _userMeasurementService.SaveMeasureAsync(userId, input);

        TempData["ValuesUpdated"] = "Ölçümleriniz güncellendi.";
        
        return RedirectToAction("Values");
    }
    private string? BuildDiseasesCsv(List<string>? selected)
    {
        if (selected == null || selected.Count == 0)
            return null;

        if (selected.Any(s => string.Equals(s, "None", StringComparison.OrdinalIgnoreCase)))
            return "None";

        var cleaned = selected
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        if (cleaned.Count == 0)
            return null;

        return string.Join(", ", cleaned);
    }


    // Yeni Program – GET: formu son ölçümle doldur
    [HttpGet]
    public async Task<IActionResult> NewProgram()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dto = await _userMeasurementService.GetMeasureAsync(userId);
        var model = new MeasurementViewModel
        {
            SelectedDiseases = new List<string>()
        };

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

            if (!string.IsNullOrWhiteSpace(dto.Diseases))
            {
                model.SelectedDiseases = dto.Diseases
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewProgram(MeasurementViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var diseasesCsv = BuildDiseasesCsv(model.SelectedDiseases);

        var input = new UserMeasureInput
        {
            Gender            = model.Gender,
            Age               = model.Age,
            HeightCm          = model.HeightCm,
            WeightKg          = model.WeightKg,
            Diseases          = diseasesCsv,
            ActivityLevel     = model.ActivityLevel,
            DietaryPreference = model.DietaryPreference
        };

        // 1) Ölçümü kaydet → Id'yi al
        int userMeasureId = await _userMeasurementService.SaveMeasureAsync(userId, input);

        // 2) Bu ölçüm için AI'dan top3 seçenekleri al
        var optionsDto = await _dietPlanService.GetDietOptionsForMeasureAsync(userMeasureId);

        var vm = new DietOptionViewModel
        {
            UserMeasureId    = optionsDto.UserMeasureId,
            BreakfastOptions = optionsDto.BreakfastOptions
                .Select(o => new SingleMealOptionViewModel
                {
                    Label = o.Label,
                    Probability = o.Probability
                }).ToList(),
            LunchOptions = optionsDto.LunchOptions
                .Select(o => new SingleMealOptionViewModel
                {
                    Label = o.Label,
                    Probability = o.Probability
                }).ToList(),
            DinnerOptions = optionsDto.DinnerOptions
                .Select(o => new SingleMealOptionViewModel
                {
                    Label = o.Label,
                    Probability = o.Probability
                }).ToList(),
            SnackOptions = optionsDto.SnackOptions
                .Select(o => new SingleMealOptionViewModel
                {
                    Label = o.Label,
                    Probability = o.Probability
                }).ToList()
        };

        // 3) Pop-up görünümüne geç (tasarımı modal gibi yapacağız)
        return View("ChooseDietOptions", vm);
        
    }
    // confirm kısmı 3 çeşit diet listesinden
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDietOptions(DietOptionViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.SelectedBreakfast) ||
            string.IsNullOrWhiteSpace(model.SelectedLunch) ||
            string.IsNullOrWhiteSpace(model.SelectedDinner) ||
            string.IsNullOrWhiteSpace(model.SelectedSnack))
        {
            ModelState.AddModelError(string.Empty, "Lütfen her öğün için bir seçenek seçin.");
            return View("ChooseDietOptions", model);
        }

        await _dietPlanService.CreateDietPlanFromChoicesAsync(
            model.UserMeasureId,
            model.SelectedBreakfast,
            model.SelectedLunch,
            model.SelectedDinner,
            model.SelectedSnack);

        TempData["ProgramCreated"] = "Yeni programınız oluşturuldu.";
        return RedirectToAction("History");
    }



    // Geçmiş programlar
    [HttpGet]
    public async Task<IActionResult> History(int page = 1)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        const int pageSize = 5;

        var plans = await _dietPlanService.GetUserPlansAsync(userId);
        var orderedPlans = plans.OrderByDescending(x => x.GeneratedAt).ToList();

        var totalPlans = orderedPlans.Count;
        var totalPages = totalPlans == 0
            ? 1
            : (int)Math.Ceiling(totalPlans / (double)pageSize);

        page = Math.Clamp(page, 1, totalPages);

        var pagedPlans = orderedPlans
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new DietPlanViewModel
            {
                Id          = p.Id,
                Breakfast   = p.Breakfast,
                Lunch       = p.Lunch,
                Dinner      = p.Dinner,
                Snack       = p.Snack,
                GeneratedAt = p.GeneratedAt
            }).ToList();
        
        var model = new DietHistoryViewModel
        {
            Plans       = pagedPlans,
            CurrentPage = totalPlans == 0 ? 0 : page,
            TotalPages  = totalPlans == 0 ? 0 : totalPages,
            PageSize    = pageSize,
            TotalPlans  = totalPlans
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
    public async Task<IActionResult> DeletePlan(int id, int page = 1)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var deleted = await _dietPlanService.DeleteUserPlanAsync(userId, id);

        if (!deleted)
            return NotFound();

        TempData["PlanDeleted"] = "Plan listeden kaldırıldı.";

        return RedirectToAction("History", new { page });
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
