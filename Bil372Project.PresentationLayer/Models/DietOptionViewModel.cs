namespace Bil372Project.PresentationLayer.Models;

public class SingleMealOptionViewModel
{
    public string Label { get; set; } = string.Empty;
    public double Probability { get; set; }
}

public class DietOptionViewModel
{
    public int UserMeasureId { get; set; }

    public List<SingleMealOptionViewModel> BreakfastOptions { get; set; } = new();
    public List<SingleMealOptionViewModel> LunchOptions { get; set; } = new();
    public List<SingleMealOptionViewModel> DinnerOptions { get; set; } = new();
    public List<SingleMealOptionViewModel> SnackOptions { get; set; } = new();

    // Kullanıcının seçimleri (radio value'lar)
    public string? SelectedBreakfast { get; set; }
    public string? SelectedLunch { get; set; }
    public string? SelectedDinner { get; set; }
    public string? SelectedSnack { get; set; }
}