namespace Bil372Project.BusinessLayer.Dtos;

public class DashboardStatsDto
{
    public double? CurrentWeightKg { get; set; }
    public double? CurrentBmi { get; set; }
    public double? WeightChangeLastMonthKg { get; set; }
    public string? BmiCategory { get; set; }
}