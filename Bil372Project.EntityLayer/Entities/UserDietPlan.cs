namespace Bil372Project.EntityLayer.Entities;

public class UserDietPlan
{
    public int Id { get; set; }

    public int UserMeasureId { get; set; }

    public string Breakfast { get; set; } = string.Empty;
    public string Lunch { get; set; } = string.Empty;
    public string Dinner { get; set; } = string.Empty;
    public string Snack { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string? ModelVersion { get; set; }  // istersen hangi model ile üretildi

    // Navigation
    public UserMeasure UserMeasure { get; set; } = null!;
}
