using RecipeApi.Models;

namespace RecipeApi.Services;

public class ScaledIngredient
{
    public string IngredientName { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public interface IPortionScalerService
{
    /// <summary>
    /// Bir tarifin malzeme miktarlarını istenen porsiyon sayısına göre ölçekler.
    /// </summary>
    List<ScaledIngredient> Scale(Recipe recipe, int targetServings);
}
