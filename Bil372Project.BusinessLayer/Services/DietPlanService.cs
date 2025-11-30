using System.Data;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class DietPlanService : IDietPlanService
{
    private readonly AppDbContext _context;
    private readonly IModelInputService _modelInputService;
    private readonly IAiDietService _aiDietService;
    private readonly IAuditLogService _auditLogService;

    public DietPlanService(
        AppDbContext context,
        IModelInputService modelInputService,
        IAiDietService aiDietService,
        IAuditLogService auditLogService)
    {
        _context = context;
        _modelInputService = modelInputService;
        _aiDietService = aiDietService;
        _auditLogService = auditLogService;
    }

    public async Task<DietOptionsDto> GetDietOptionsForMeasureAsync(int userMeasureId)
    {
        var input = await _modelInputService.BuildModelInputAsync(userMeasureId);
        if (input == null)
            throw new Exception("Ölçüm veya ML verileri bulunamadı.");

        return await _aiDietService.GetDietOptionsAsync(input, userMeasureId);
    }

    public async Task<UserDietPlan> CreateDietPlanFromChoicesAsync(
        int userMeasureId,
        string breakfast,
        string lunch,
        string dinner,
        string snack)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        
        var plan = new UserDietPlan
        {
            UserMeasureId = userMeasureId,
            Breakfast     = breakfast,
            Lunch         = lunch,
            Dinner        = dinner,
            Snack         = snack,
            GeneratedAt   = DateTime.UtcNow,
            ModelVersion  = "rf_v1"
        };

        _context.UserDietPlans.Add(plan);
        await _context.SaveChangesAsync();
        
        var measureOwnerId = await _context.UserMeasures
            .Where(m => m.Id == userMeasureId)
            .Select(m => (int?)m.UserId)
            .FirstOrDefaultAsync();

        await _auditLogService.LogAsync(measureOwnerId, "UserDietPlans", "Insert", null, new
        {
            plan.Id,
            plan.UserMeasureId,
            plan.GeneratedAt,
            plan.ModelVersion
        });

        await transaction.CommitAsync();
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
