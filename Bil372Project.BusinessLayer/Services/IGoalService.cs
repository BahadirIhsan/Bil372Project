using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public interface IGoalService
{
    Task<UserGoalDto?> GetLatestGoalAsync(int userId);
    Task<int> SaveGoalAsync(int userId, UserGoalInput input);
    Task<int?> IncrementWaterAsync(int userId, int increment = 1);

}