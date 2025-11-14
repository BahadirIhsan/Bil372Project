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
                DislikedFoods = measure.DislikedFoods
            };
        }

        Task IUserMeasurementService.SaveMeasureAsync(int userId, UserMeasureInput input)
        {
            return SaveMeasureAsync(userId, input);
        }

        public async Task SaveMeasureAsync(int userId, UserMeasureInput input)
        {
            var measure = await _context.UserMeasures
                .FirstOrDefaultAsync(m => m.UserId == userId);

            // BMI hesabı burada
            var bmi = input.WeightKg / Math.Pow(input.HeightCm / 100.0, 2);

            if (measure == null)
            {
                // Yeni kayıt
                measure = new UserMeasure
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
            }
            else
            {
                // Güncelleme
                measure.Gender        = input.Gender;
                measure.Age           = input.Age;
                measure.HeightCm      = input.HeightCm;
                measure.WeightKg      = input.WeightKg;
                measure.Allergies     = input.Allergies;
                measure.Diseases      = input.Diseases;
                measure.DislikedFoods = input.DislikedFoods;
                measure.Bmi           = bmi;
                measure.UpdatedAt     = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        
    }