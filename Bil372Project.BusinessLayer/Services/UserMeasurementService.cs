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
                .FirstOrDefaultAsync(m => m.UserId == userId);

            if (measure == null)
                return null;

            return new UserMeasureInput
            {
                Gender        = measure.Gender,
                Age           = measure.Age,
                HeightCm      = measure.HeightCm,
                WeightKg      = measure.WeightKg,
                Allergies     = measure.Allergies,
                Diseases      = measure.Diseases,
                DislikedFoods = measure.DislikedFoods,
                LastUpdatedAt = measure.UpdatedAt

            };
        }

        Task IUserMeasurementService.SaveMeasureAsync(int userId, UserMeasureInput input)
        {
            return SaveMeasureAsync(userId, input);
        }

        public async Task SaveMeasureAsync(int userId, UserMeasureInput input)
        {
            //var measure = await _context.UserMeasures.FirstOrDefaultAsync(m => m.UserId == userId);

            // BMI hesabı burada
            var bmi = input.WeightKg / Math.Pow(input.HeightCm / 100.0, 2);
            
                // Yeni kayıt
            var measure = new UserMeasure
            {
                UserId        = userId,
                Gender        = input.Gender,
                Age           = input.Age,
                HeightCm      = input.HeightCm,
                WeightKg      = input.WeightKg,
                Allergies     = input.Allergies,
                Diseases      = input.Diseases,
                DislikedFoods = input.DislikedFoods,
                Bmi           = bmi,
                UpdatedAt     = DateTime.UtcNow
            };

            _context.UserMeasures.Add(measure);
            await _context.SaveChangesAsync();
            
        }
        
        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
        {
            var stats = new DashboardStatsDto();

            var nowUtc = DateTime.UtcNow;

            // En son ölçüm
            var latest = await _context.UserMeasures
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.UpdatedAt)
                .FirstOrDefaultAsync();

            if (latest == null)
                return stats; // hiç veri yok → hepsi null

            stats.CurrentWeightKg = latest.WeightKg;
            stats.CurrentBmi      = latest.Bmi;

            // BMI kategori
            stats.BmiCategory = GetBmiCategory(latest.Bmi);

            // Yaklaşık 1 ay önceki ölçüm
            // 1 ay içindeki tüm ölçümler
            var oneMonthStart = nowUtc.AddMonths(-1);

            var lastMonthMeasures = await _context.UserMeasures
                .Where(m => m.UserId == userId && m.UpdatedAt >= oneMonthStart)
                .OrderBy(m => m.UpdatedAt)          // 🔹 en eskiden yeniye
                .ToListAsync();

            // Eğer 1 ay içinde hiç ölçüm yoksa → değişim yok
            if (lastMonthMeasures.Count > 0)
            {
                var oldestInMonth = lastMonthMeasures.First();   // 🔹 1 ay içindeki en eski ölçüm
                stats.WeightChangeLastMonthKg = latest.WeightKg - oldestInMonth.WeightKg;
            }
            else
            {
                stats.WeightChangeLastMonthKg = null;
            }

            return stats;
        }

        // Helper: BMI'ye göre kategori
        private string GetBmiCategory(double bmi)
        {
            if (bmi < 18.5) return "Zayıf";
            if (bmi < 25)   return "Sağlıklı aralıkta";
            if (bmi < 30)   return "Fazla kilolu";

            return "Obez";
        }


        
    }