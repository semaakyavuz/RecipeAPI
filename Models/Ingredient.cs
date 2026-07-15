namespace RecipeApi.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // 1 birim ingredient'ın kaç kalori olduğu (basit besin bilgisi için)
    public double CaloriesPerUnit { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
