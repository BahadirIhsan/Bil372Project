namespace Bil372Project.EntityLayer.Entities;

public class UserMeasure
{
    public int Id { get; set; }

    // Hangi kullanıcıya ait?
    public int UserId { get; set; }

    // Temel bilgiler
    public string Gender { get; set; } = string.Empty;   // "Male/Female" ya da "Erkek/Kadın" string
    public int Age { get; set; }

    public double HeightCm { get; set; }
    public double WeightKg { get; set; }

    // Kullanıcıdan alınan text alanları
    public string Allergies { get; set; } = string.Empty;        // "Fıstık, süt ürünleri..." gibi
    public string Diseases { get; set; } = string.Empty;         // "Diyabet, hipertansiyon..."
    public string DislikedFoods { get; set; } = string.Empty;    // "Balık, brokoli..."

    // Kullanıcının görmediği ama bizim hesapladığımız değer
    public double Bmi { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation (istersen)
    public AppUser User { get; set; } = null!;

}