namespace RecipeApi.Services;

public class NutritionInfo
{
    public double TotalCalories { get; set; }
    public string Source { get; set; } = string.Empty;
}

public interface INutritionInfoService
{
    /// <summary>
    /// Verilen malzeme adı ve miktara göre (dış bir API'den) besin bilgisi getirir.
    /// Gerçek implementasyon harici bir servise HTTP çağrısı yapar; testlerde Moq ile mock'lanır.
    /// </summary>
    Task<NutritionInfo> GetNutritionInfoAsync(string ingredientName, double amount, string unit);
}
