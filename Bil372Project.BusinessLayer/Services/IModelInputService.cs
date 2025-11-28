using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public interface IModelInputService
{
    Task<ModelInput?> BuildModelInputAsync(int userMeasureId);
}