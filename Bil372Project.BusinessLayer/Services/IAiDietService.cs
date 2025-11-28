using System.Threading.Tasks;
using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public interface IAiDietService
{
    Task<(string Breakfast, string Lunch, string Dinner, string Snack)> 
        GenerateDietAsync(ModelInput input);
}