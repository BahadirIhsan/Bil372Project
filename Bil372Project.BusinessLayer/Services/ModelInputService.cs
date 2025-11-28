using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class ModelInputService : IModelInputService
{
    private readonly AppDbContext _context;

    public ModelInputService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ModelInput?> BuildModelInputAsync(int userMeasureId)
    {
        // Ölçüm + ML hesaplarını birlikte çek
        var measure = await _context.UserMeasures
            .Include(m => m.MeasurementForMl)
            .FirstOrDefaultAsync(m => m.Id == userMeasureId);

        if (measure == null || measure.MeasurementForMl == null)
            return null;

        return new ModelInput
        {
            Gender = measure.Gender,
            ActivityLevel = measure.ActivityLevel,
            DietaryPreference = measure.DietaryPreference,
            Disease = measure.Diseases,
            Ages = measure.Age,
            Height = measure.HeightCm,
            Weight = measure.WeightKg,

            DailyCalorieTarget = measure.MeasurementForMl.DailyCalorieTarget,
            Protein = measure.MeasurementForMl.ProteinGrams,
            Fat = measure.MeasurementForMl.FatGrams,
            Sugar = measure.MeasurementForMl.SugarGrams,
            Sodium = measure.MeasurementForMl.SodiumMg / 1000.0, // mg to g
        };
    }
}