using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.EntityLayer.Entities;

namespace Bil372Project.BusinessLayer.Services;

public interface IDietPlanService
{
    Task<DietOptionsDto> GetDietOptionsForMeasureAsync(int userMeasureId);

    Task<UserDietPlan> CreateDietPlanFromChoicesAsync(
        int userMeasureId,
        string breakfast,
        string lunch,
        string dinner,
        string snack);

    Task<IList<UserDietPlan>> GetUserPlansAsync(int userId);
}

