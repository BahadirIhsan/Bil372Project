namespace Bil372Project.PresentationLayer.Models;

public class DietHistoryViewModel
{
    public IList<DietPlanViewModel> Plans { get; set; } = new List<DietPlanViewModel>();
    
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalPlans { get; set; }
}