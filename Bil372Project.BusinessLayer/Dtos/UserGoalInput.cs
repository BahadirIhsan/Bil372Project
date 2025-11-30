namespace Bil372Project.BusinessLayer.Dtos;

public class UserGoalInput
{
    public double? TargetWeightKg { get; set; }
    public int? GoalDurationWeeks { get; set; }
    public int? DailyWaterTarget { get; set; }
    public int? WaterConsumedToday { get; set; }
    public int? WeeklyActivityTarget { get; set; }
    public string? MotivationNote { get; set; }
}