namespace Bil372Project.EntityLayer.Entities;

public class UserGoal
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public double? TargetWeightKg { get; set; }
    public int? GoalDurationWeeks { get; set; }

    public int? DailyWaterTarget { get; set; }
    public int? WaterConsumedToday { get; set; }

    public int? WeeklyActivityTarget { get; set; }

    public string? MotivationNote { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
}