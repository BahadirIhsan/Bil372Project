namespace Bil372Project.PresentationLayer.Models;

public class TodayDietPlanViewModel
{
    public string? Breakfast { get; set; }
    public string? Lunch { get; set; }
    public string? Dinner { get; set; }
    public string? Snack { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class DashboardViewModel
{
    public double? CurrentWeightKg { get; set; }
    public double? WeightChangeLastMonthKg { get; set; }
    public double? CurrentBmi { get; set; }
    public string? BmiCategory { get; set; }
    public TodayDietPlanViewModel? TodayPlan { get; set; }


    public bool HasData => CurrentWeightKg.HasValue && CurrentBmi.HasValue;
}