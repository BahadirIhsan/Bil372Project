namespace Bil372Project.EntityLayer.Entities;

public class UserMeasure
{
    public int Id { get; set; }

    // Hangi kullanıcıya ait?
    public int UserId { get; set; }

    // Temel bilgiler
    public string Gender { get; set; } = string.Empty;   // "Male/Female" ya da "Erkek/Kadın"
    public int Age { get; set; }

    public double HeightCm { get; set; }
    public double WeightKg { get; set; }

    // Kullanıcının seçtiği yaşam tarzı / diyet türü
    public string ActivityLevel { get; set; } = string.Empty;      // "Sedentary", "Active" vb.
    public string DietaryPreference { get; set; } = string.Empty;  // "Vegan", "Keto" vb.

    // Kullanıcıdan alınan text alanları
    public string? Diseases { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = null!;

    // 1-1: Hesaplanan değerler (BMI, kalori, makrolar)
    public MeasurementForMl MeasurementForMl { get; set; } = null!;

    // 1-n: Bu ölçüden üretilmiş diyet planları
    public ICollection<UserDietPlan> DietPlans { get; set; } = new List<UserDietPlan>();
}