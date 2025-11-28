namespace Bil372Project.PresentationLayer.Models;

public class DietPlanViewModel
{
    public int Id { get; set; }
    public DateTime GeneratedAt { get; set; }

    public string Breakfast { get; set; } = string.Empty;
    public string Lunch { get; set; } = string.Empty;
    public string Dinner { get; set; } = string.Empty;
    public string Snack { get; set; } = string.Empty;
}