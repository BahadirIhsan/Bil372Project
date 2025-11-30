using System.Data;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class GoalService : IGoalService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public GoalService(AppDbContext context, IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<UserGoalDto?> GetLatestGoalAsync(int userId)
    {
        var goal = await _context.UserGoals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.UpdatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (goal == null)
            return null;

        return new UserGoalDto
        {
            TargetWeightKg = goal.TargetWeightKg,
            GoalDurationWeeks = goal.GoalDurationWeeks,
            DailyWaterTarget = goal.DailyWaterTarget,
            WaterConsumedToday = goal.WaterConsumedToday,
            WeeklyActivityTarget = goal.WeeklyActivityTarget,
            MotivationNote = goal.MotivationNote,
            UpdatedAt = goal.UpdatedAt
        };
    }

    public async Task<int> SaveGoalAsync(int userId, UserGoalInput input)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var goal = new UserGoal
        {
            UserId = userId,
            TargetWeightKg = input.TargetWeightKg,
            GoalDurationWeeks = input.GoalDurationWeeks,
            DailyWaterTarget = input.DailyWaterTarget,
            WaterConsumedToday = input.WaterConsumedToday,
            WeeklyActivityTarget = input.WeeklyActivityTarget,
            MotivationNote = string.IsNullOrWhiteSpace(input.MotivationNote) ? null : input.MotivationNote.Trim(),
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserGoals.Add(goal);
        await _context.SaveChangesAsync();
        
        await _auditLogService.LogAsync(userId, "UserGoals", "Insert", null, new
        {
            goal.TargetWeightKg,
            goal.GoalDurationWeeks,
            goal.DailyWaterTarget,
            goal.WaterConsumedToday,
            goal.WeeklyActivityTarget,
            goal.MotivationNote,
            goal.UpdatedAt
        });

        await transaction.CommitAsync();
        
        return goal.Id;
    }
    
    public async Task<int?> IncrementWaterAsync(int userId, int increment = 1)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        
        var latestGoal = await _context.UserGoals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.UpdatedAt)
            .FirstOrDefaultAsync();

        if (latestGoal == null)
        {
            await transaction.RollbackAsync();
            return null;
        }
        
        var previousWater = latestGoal.WaterConsumedToday;
        
        latestGoal.WaterConsumedToday = (latestGoal.WaterConsumedToday ?? 0) + increment;
        latestGoal.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(userId, "UserGoals", "Update", new
                {
                    WaterConsumedToday = previousWater,
                    latestGoal.UpdatedAt
                },
                new
                {
                    WaterConsumedToday = latestGoal.WaterConsumedToday,
                    latestGoal.UpdatedAt
                });

            await transaction.CommitAsync();

            return latestGoal.WaterConsumedToday;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            return null;
        }

    }
}