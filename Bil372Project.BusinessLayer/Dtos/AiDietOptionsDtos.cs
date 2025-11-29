using System.Collections.Generic;

namespace Bil372Project.BusinessLayer.Dtos;

public class MealOptionDto
{
    public string Label { get; set; } = string.Empty;
    public double Probability { get; set; }
}

public class DietOptionsDto
{
    public int UserMeasureId { get; set; }

    public List<MealOptionDto> BreakfastOptions { get; set; } = new();
    public List<MealOptionDto> LunchOptions { get; set; } = new();
    public List<MealOptionDto> DinnerOptions { get; set; } = new();
    public List<MealOptionDto> SnackOptions { get; set; } = new();
}