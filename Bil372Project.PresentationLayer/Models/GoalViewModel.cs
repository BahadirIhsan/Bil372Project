using System.ComponentModel.DataAnnotations;

namespace Bil372Project.PresentationLayer.Models;

public class GoalViewModel
{
    [Display(Name = "Hedef Kilo (kg)")]
    [Range(1, 500, ErrorMessage = "Lütfen geçerli bir değer girin.")]
    public double? TargetWeightKg { get; set; }

    [Display(Name = "Hedef Süre (hafta)")]
    [Range(1, 104, ErrorMessage = "1-104 hafta arası bir değer girin.")]
    public int? GoalDurationWeeks { get; set; }

    [Display(Name = "Günlük Su Hedefi (bardak)")]
    [Range(1, 30, ErrorMessage = "1-30 bardak arası bir değer girin.")]
    public int? DailyWaterTarget { get; set; }

    [Display(Name = "Bugün İçtiğin Su (bardak)")]
    [Range(0, 30, ErrorMessage = "0-30 bardak arası bir değer girin.")]
    public int? WaterConsumedToday { get; set; }

    [Display(Name = "Haftalık Aktivite Hedefi (seans)")]
    [Range(0, 20, ErrorMessage = "0-20 seans arası bir değer girin.")]
    public int? WeeklyActivityTarget { get; set; }

    [Display(Name = "Motivasyon Notu")]
    [MaxLength(500, ErrorMessage = "En fazla 500 karakter girebilirsiniz.")]
    public string? MotivationNote { get; set; }

    public DateTime? UpdatedAt { get; set; }
}