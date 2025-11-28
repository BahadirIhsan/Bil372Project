using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public class AiDietService : IAiDietService
{
    public Task<(string Breakfast, string Lunch, string Dinner, string Snack)> 
        GenerateDietAsync(ModelInput input)
    {
        // Şimdilik sahte değer döndürelim
        return Task.FromResult((
            Breakfast: "Yulaf + süt",
            Lunch: "Izgara tavuk + salata",
            Dinner: "Sebzeli omlet",
            Snack: "Meyve + badem"
        ));
    }
}