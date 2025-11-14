namespace Bil372Project.BusinessLayer.Dtos;

public class UserMeasureInput
{
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public string Allergies { get; set; } = string.Empty;
    public string Diseases { get; set; } = string.Empty;
    public string DislikedFoods { get; set; } = string.Empty;
    
    public DateTime? LastUpdatedAt { get; set; }

}