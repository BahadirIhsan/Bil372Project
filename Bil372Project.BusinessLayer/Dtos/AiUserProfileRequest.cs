using System.Text.Json.Serialization;

namespace Bil372Project.BusinessLayer.Dtos;

// Python tarafındaki UserProfile şemasına karşılık
public class AiUserProfileRequest
{
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = "";

    [JsonPropertyName("activity_level")]
    public string ActivityLevel { get; set; } = "";

    [JsonPropertyName("dietary_preference")]
    public string DietaryPreference { get; set; } = "";

    [JsonPropertyName("disease")]
    public string Disease { get; set; } = "";

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonPropertyName("daily_calorie_target")]
    public double DailyCalorieTarget { get; set; }

    [JsonPropertyName("protein")]
    public double Protein { get; set; }

    [JsonPropertyName("fat")]
    public double Fat { get; set; }

    [JsonPropertyName("sugar")]
    public double Sugar { get; set; }

    // Python şemasında "sodium" mg cinsinden
    [JsonPropertyName("sodium")]
    public double SodiumMg { get; set; }
}

// Python’ın döndürdüğü JSON
public class AiMealTop
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("prob")]
    public double Prob { get; set; }
}

public class AiMealResponse
{
    [JsonPropertyName("meal_type")]
    public string MealType { get; set; } = "";

    [JsonPropertyName("top1")]
    public AiMealTop Top1 { get; set; } = new();

    [JsonPropertyName("top3")]
    public List<AiMealTop> Top3 { get; set; } = new();
}