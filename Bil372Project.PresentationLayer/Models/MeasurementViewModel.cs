using System.ComponentModel.DataAnnotations;

namespace Bil372Project.PresentationLayer.Models;

public class MeasurementViewModel
{
    [Required(ErrorMessage = "Cinsiyet zorunludur")]
    public string Gender { get; set; } = string.Empty;

    [Range(1, 120, ErrorMessage = "Yaş 1 ile 120 arasında olmalıdır")]
    public int Age { get; set; }

    [Range(30, 250, ErrorMessage = "Boy 30 ile 250 cm arasında olmalıdır")]
    [Display(Name = "Boy (cm)")]
    public double HeightCm { get; set; }

    [Range(10, 400, ErrorMessage = "Kilo 10 ile 400 kg arasında olmalıdır")]
    [Display(Name = "Kilo (kg)")]
    public double WeightKg { get; set; }

    public string Allergies { get; set; } = string.Empty;
    public string Diseases { get; set; } = string.Empty;
    public string DislikedFoods { get; set; } = string.Empty;

    public DateTime? LastUpdatedAt { get; set; }
    public string? LastUpdatedText { get; set; }
}
