using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class UserMeasurementService : IUserMeasurementService
{
    private readonly AppDbContext _context;

    public UserMeasurementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserMeasureInput?> GetMeasureAsync(int userId)
    {
        var measure = await _context.UserMeasures
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.UpdatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (measure == null)
            return null;

        return new UserMeasureInput
        {
            Gender        = measure.Gender,
            Age           = measure.Age,
            HeightCm      = measure.HeightCm,
            WeightKg      = measure.WeightKg,
            Diseases      = measure.Diseases,
            // İleride ActivityLevel / DietaryPreference ekleriz
            LastUpdatedAt = measure.UpdatedAt
        };
    }

    Task <int> IUserMeasurementService.SaveMeasureAsync(int userId, UserMeasureInput input)
    {
        return SaveMeasureAsync(userId, input);
    }

    public async Task <int> SaveMeasureAsync(int userId, UserMeasureInput input)
    {
        // 1) Önce UserMeasure kaydını oluştur
        var measure = new UserMeasure
        {
            UserId        = userId,
            Gender        = input.Gender,
            Age           = input.Age,
            HeightCm      = input.HeightCm,
            WeightKg      = input.WeightKg,
            Diseases      = string.IsNullOrWhiteSpace(input.Diseases)  ? null : input.Diseases,
            ActivityLevel = input.ActivityLevel,        // yeni
            DietaryPreference = input.DietaryPreference,// yeni
            UpdatedAt     = DateTime.UtcNow
        };

        _context.UserMeasures.Add(measure);
        await _context.SaveChangesAsync();   // 🔹 measure.Id burada oluşur

        // 2) BMI Hesabı
        double bmi = measure.WeightKg / Math.Pow(measure.HeightCm / 100.0, 2);

        // 3) Günlük kalori + makrolar (örnek formüller)
        double dailyCalorie = CalculateDailyCalorie(measure);
        var macros = CalculateMacros(dailyCalorie);

        // 4) MeasurementForMl kaydını oluştur
        var ml = new MeasurementForMl
        {
            UserMeasureId      = measure.Id,
            Bmi                = bmi,
            DailyCalorieTarget = dailyCalorie,
            ProteinGrams       = macros.Protein,
            FatGrams           = macros.Fat,
            SugarGrams         = macros.Sugar,
            SodiumMg           = macros.Sodium,
            CalculatedAt       = DateTime.UtcNow
        };

        _context.MeasurementsForMl.Add(ml);
        await _context.SaveChangesAsync();
        
        return measure.Id;

    }
        
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
    {
        var stats = new DashboardStatsDto();
        var nowUtc = DateTime.UtcNow;

        // En son ölçüm + ML hesapları
        var latest = await _context.UserMeasures
            .Include(m => m.MeasurementForMl)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.UpdatedAt)
            .FirstOrDefaultAsync();

        if (latest == null)
            return stats; // hiç veri yok

        stats.CurrentWeightKg = latest.WeightKg;

        // BMI ve kategori
        if (latest.MeasurementForMl != null)
        {
            stats.CurrentBmi = latest.MeasurementForMl.Bmi;
            stats.BmiCategory = GetBmiCategory(stats.CurrentBmi.Value);
        }

        // Son 1 ay içindeki kilo değişimi
        var oneMonthStart = nowUtc.AddMonths(-1);

        var lastMonthMeasures = await _context.UserMeasures
            .Where(m => m.UserId == userId && m.UpdatedAt >= oneMonthStart)
            .OrderBy(m => m.UpdatedAt)
            .ToListAsync();

        if (lastMonthMeasures.Count > 0)
        {
            var oldestInMonth = lastMonthMeasures.First();
            stats.WeightChangeLastMonthKg = latest.WeightKg - oldestInMonth.WeightKg;
        }
        else
        {
            stats.WeightChangeLastMonthKg = null;
        }

        return stats;
    }


    // BMI kategori istersen DashboardStatsDto'ya property ekleyince kullanabiliriz
    private string GetBmiCategory(double bmi)
    {
        if (bmi < 18.5) return "Zayıf";
        if (bmi < 25)   return "Sağlıklı aralıkta";
        if (bmi < 30)   return "Fazla kilolu";
        return "Obez";
    }

    // ÖRNEK kalori formülü (ileride değiştirebiliriz)
    private double CalculateDailyCalorie(UserMeasure m)
    {
        double bmr = 10 * m.WeightKg + 6.25 * m.HeightCm - 5 * m.Age + (m.Gender == "Male" ? 5 : -161);

        double factor = m.ActivityLevel switch
        {
            "Sedentary"         => 1.2,
            "Lightly Active"    => 1.375,
            "Moderately Active" => 1.55,
            "Very Active"       => 1.725,
            "Extremely Active"  => 1.9,
            _                   => 1.2
        };

        return bmr * factor;
    }


    private (double Protein, double Fat, double Sugar, double Sodium) CalculateMacros(double cal)
    {
        double protein = (cal * 0.30) / 4; // 1g protein = 4 cal
        double fat     = (cal * 0.25) / 9; // 1g fat = 9 cal
        double sugar   = (cal * 0.45) / 4; // 1g carb ≈ 4 cal
        double sodium  = 1500;             // sabit örnek

        return (protein, fat, sugar, sodium);
    }
}
