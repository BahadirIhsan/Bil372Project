using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class DietPlanService : IDietPlanService
{
    private readonly AppDbContext _context;
    private readonly IModelInputService _modelInputService;
    private readonly IAiDietService _aiDietService;

    public DietPlanService(
        AppDbContext context,
        IModelInputService modelInputService,
        IAiDietService aiDietService)
    {
        _context = context;
        _modelInputService = modelInputService;
        _aiDietService = aiDietService;
    }

    public async Task<UserDietPlan> CreateDietPlanAsync(int userMeasureId)
    {
        // 1) ML input oluştur
        var input = await _modelInputService.BuildModelInputAsync(userMeasureId);

        if (input == null)
            throw new Exception("Ölçüm veya ML verileri bulunamadı.");

        // 2) AI modeli çağır
        var aiResult = await _aiDietService.GenerateDietAsync(input);

        // 3) DietPlan kaydı oluştur
        var plan = new UserDietPlan
        {
            UserMeasureId = userMeasureId,
            Breakfast = aiResult.Breakfast,
            Lunch = aiResult.Lunch,
            Dinner = aiResult.Dinner,
            Snack = aiResult.Snack,
            GeneratedAt = DateTime.UtcNow,
            ModelVersion = "v1"
        };

        _context.UserDietPlans.Add(plan);
        await _context.SaveChangesAsync();

        return plan;
    }

    public async Task<IList<UserDietPlan>> GetUserPlansAsync(int userId)
    {
        return await _context.UserDietPlans
            .Include(p => p.UserMeasure)
            .Where(p => p.UserMeasure.UserId == userId)
            .OrderByDescending(p => p.GeneratedAt)
            .ToListAsync();
    }
}