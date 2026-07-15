using RecipeApi.Models;

namespace RecipeApi.Services;

public class PortionScalerService : IPortionScalerService
{
    public List<ScaledIngredient> Scale(Recipe recipe, int targetServings)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (targetServings <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetServings), "Porsiyon sayısı 0'dan büyük olmalı.");
        }

        if (recipe.BaseServings <= 0)
        {
            throw new InvalidOperationException("Tarifin BaseServings değeri 0'dan büyük olmalı.");
        }

        double scaleFactor = (double)targetServings / recipe.BaseServings;

        return recipe.RecipeIngredients
            .Select(ri => new ScaledIngredient
            {
                IngredientName = ri.Ingredient?.Name ?? string.Empty,
                Amount = Math.Round(ri.Amount * scaleFactor, 2),
                Unit = ri.Unit
            })
            .ToList();
    }
}
