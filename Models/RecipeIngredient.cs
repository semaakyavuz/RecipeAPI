namespace RecipeApi.Models;

public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    public int IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }

    // BaseServings için gereken miktar (örn. 200 gr, 2 adet)
    public double Amount { get; set; }
    public string Unit { get; set; } = string.Empty;
}
