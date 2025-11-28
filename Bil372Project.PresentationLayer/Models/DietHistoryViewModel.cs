namespace Bil372Project.PresentationLayer.Models;

public class DietHistoryViewModel
{
    public IList<DietPlanViewModel> Plans { get; set; } = new List<DietPlanViewModel>();
}