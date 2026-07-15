namespace RecipeApi.Services;

// Gerçek projede burada HttpClient ile üçüncü parti bir Nutrition API'sine (örn. Edamam, Nutritionix) istek atılır.
// Bu proje kapsamında basit bir simülasyon; asıl amaç arayüzün (INutritionInfoService) test edilebilir/mock'lanabilir olması.
public class NutritionInfoService : INutritionInfoService
{
    public async Task<NutritionInfo> GetNutritionInfoAsync(string ingredientName, double amount, string unit)
    {
        await Task.Delay(50); // dış API çağrısını simüle eder

        return new NutritionInfo
        {
            TotalCalories = amount * 1.5, // placeholder hesap
            Source = "external-nutrition-api"
        };
    }
}
