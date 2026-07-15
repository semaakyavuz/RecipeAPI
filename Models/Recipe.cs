namespace RecipeApi.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;

    // Bu tarifin yazıldığı orijinal porsiyon sayısı (ölçekleme hesaplarının temeli)
    public int BaseServings { get; set; } = 4;

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
