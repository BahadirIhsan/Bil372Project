namespace Bil372Project.BusinessLayer.Dtos;

public class UserMeasureInput
{
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string ActivityLevel { get; set; }
    public string DietaryPreference { get; set; }
    public string? Diseases { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

}