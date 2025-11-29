namespace Bil372Project.BusinessLayer.Dtos;

public class ModelInput
{
    public string Gender { get; set; } = "";
    public string ActivityLevel { get; set; } = "";
    public string DietaryPreference { get; set; } = "";
    public string Diseases { get; set; } = "";  
    public int Ages { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public double DailyCalorieTarget { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Sugar { get; set; }
    public double Sodium { get; set; }
}