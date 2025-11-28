namespace Bil372Project.EntityLayer.Entities;

public class MeasurementForMl
{
    public int Id { get; set; }

    public int UserMeasureId { get; set; }

    public double Bmi { get; set; }
    public double DailyCalorieTarget { get; set; }

    public double ProteinGrams { get; set; }
    public double FatGrams { get; set; }
    public double SugarGrams { get; set; }
    public double SodiumMg { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public UserMeasure UserMeasure { get; set; } = null!;
}