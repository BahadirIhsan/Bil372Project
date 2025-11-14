namespace Bil372Project.PresentationLayer.Models;

public class DashboardViewModel
{
    public double? CurrentWeightKg { get; set; }
    public double? WeightChangeLastMonthKg { get; set; }
    public double? CurrentBmi { get; set; }
    public string? BmiCategory { get; set; }

    public bool HasData => CurrentWeightKg.HasValue && CurrentBmi.HasValue;
}