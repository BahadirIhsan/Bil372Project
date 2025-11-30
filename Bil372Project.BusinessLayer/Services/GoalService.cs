using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.Services;

public class GoalService : IGoalService
{
    private readonly AppDbContext _context;

    public GoalService(AppDbContext context)
    {
        _context = context;
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
        return goal.Id;
    }
    
    public async Task<int?> IncrementWaterAsync(int userId, int increment = 1)
    {
        var latestGoal = await _context.UserGoals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.UpdatedAt)
            .FirstOrDefaultAsync();

        if (latestGoal == null)
            return null;

        latestGoal.WaterConsumedToday = (latestGoal.WaterConsumedToday ?? 0) + increment;
        latestGoal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return latestGoal.WaterConsumedToday;
    }
}