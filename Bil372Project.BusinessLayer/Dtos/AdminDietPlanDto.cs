namespace Bil372Project.BusinessLayer.Dtos;

public class AdminDietPlanDto
{
    public int Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string Breakfast { get; set; } = string.Empty;
    public string Lunch { get; set; } = string.Empty;
    public string Dinner { get; set; } = string.Empty;
    public string Snack { get; set; } = string.Empty;
    public string? ModelVersion { get; set; }
    public DateTime GeneratedAt { get; set; }
}