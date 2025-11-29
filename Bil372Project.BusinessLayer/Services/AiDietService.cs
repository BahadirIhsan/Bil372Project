using System.Net.Http.Json;
using Bil372Project.BusinessLayer.Dtos;

namespace Bil372Project.BusinessLayer.Services;

public class AiDietService : IAiDietService
{
    private readonly HttpClient _httpClient;

    public AiDietService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(string Breakfast, string Lunch, string Dinner, string Snack)>
        GenerateDietAsync(ModelInput input)
    {
        // Disease null ya da boşsa Python'a "None" gönder
        var disease = string.IsNullOrWhiteSpace(input.Disease) ? "None" : input.Disease;

        // ModelInput.Sodium gram; Python mg bekliyor → çevir
        double sodiumMg = input.Sodium * 1000.0;

        var profile = new AiUserProfileRequest
        {
            Gender             = input.Gender,
            ActivityLevel      = input.ActivityLevel,
            DietaryPreference  = input.DietaryPreference,
            Disease            = disease,
            Age                = input.Ages,
            Height             = input.Height,
            Weight             = input.Weight,
            DailyCalorieTarget = input.DailyCalorieTarget,
            Protein            = input.Protein,
            Fat                = input.Fat,
            Sugar              = input.Sugar,
            SodiumMg           = sodiumMg
        };

        var breakfast = await CallMealAsync("breakfast", profile);
        var lunch     = await CallMealAsync("lunch", profile);
        var dinner    = await CallMealAsync("dinner", profile);
        var snack     = await CallMealAsync("snack", profile);

        return (breakfast, lunch, dinner, snack);
    }

    private async Task<string> CallMealAsync(string mealType, AiUserProfileRequest profile)
    {
        var response = await _httpClient.PostAsJsonAsync($"/predict/{mealType}", profile);

        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            throw new Exception($"AI servisi hata döndü ({mealType}): {response.StatusCode} - {msg}");
        }

        var data = await response.Content.ReadFromJsonAsync<AiMealResponse>();
        if (data == null || data.Top1 == null || string.IsNullOrWhiteSpace(data.Top1.Label))
        {
            throw new Exception($"AI servisi beklenmeyen cevap döndürdü ({mealType}).");
        }

        return data.Top1.Label; // Top-1 yemek adı
    }
    
    public async Task<DietOptionsDto> GetDietOptionsAsync(ModelInput input, int userMeasureId)
{
    var disease = string.IsNullOrWhiteSpace(input.Disease) ? "None" : input.Disease;
    double sodiumMg = input.Sodium * 1000.0; // gram -> mg

    var profile = new AiUserProfileRequest
    {
        Gender             = input.Gender,
        ActivityLevel      = input.ActivityLevel,
        DietaryPreference  = input.DietaryPreference,
        Disease            = disease,
        Age                = input.Ages,
        Height             = input.Height,
        Weight             = input.Weight,
        DailyCalorieTarget = input.DailyCalorieTarget,
        Protein            = input.Protein,
        Fat                = input.Fat,
        Sugar              = input.Sugar,
        SodiumMg           = sodiumMg
    };

    var dto = new DietOptionsDto
    {
        UserMeasureId = userMeasureId,
        BreakfastOptions = await CallMealOptionsAsync("breakfast", profile),
        LunchOptions     = await CallMealOptionsAsync("lunch", profile),
        DinnerOptions    = await CallMealOptionsAsync("dinner", profile),
        SnackOptions     = await CallMealOptionsAsync("snack", profile)
    };

    return dto;
}

    private async Task<List<MealOptionDto>> CallMealOptionsAsync(
        string mealType,
        AiUserProfileRequest profile)
    {
        var response = await _httpClient.PostAsJsonAsync($"/predict/{mealType}", profile);

        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            throw new Exception($"AI servisi hata döndü ({mealType}): {response.StatusCode} - {msg}");
        }

        var data = await response.Content.ReadFromJsonAsync<AiMealResponse>();
        if (data == null || data.Top3 == null || data.Top3.Count == 0)
            throw new Exception($"AI servisi beklenmeyen cevap döndürdü ({mealType}).");

        return data.Top3
            .Select(t => new MealOptionDto
            {
                Label = t.Label,
                Probability = t.Prob
            })
            .ToList();
    }

}
