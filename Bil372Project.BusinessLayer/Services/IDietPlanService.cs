using Bil372Project.EntityLayer.Entities;

namespace Bil372Project.BusinessLayer.Services;

public interface IDietPlanService
{
    Task<UserDietPlan> CreateDietPlanAsync(int userMeasureId);
    Task<IList<UserDietPlan>> GetUserPlansAsync(int userId);

}
