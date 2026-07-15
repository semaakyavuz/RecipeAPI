using RecipeApi.Models;
using RecipeApi.Services;
using Xunit;

namespace RecipeApi.Tests.Services;

public class PortionScalerServiceTests
{
    private readonly PortionScalerService _sut = new();

    private static Recipe CreateRecipe(int baseServings, params (string name, double amount, string unit)[] items)
    {
        var recipe = new Recipe { Id = 1, Title = "Test Recipe", BaseServings = baseServings };

        foreach (var item in items)
        {
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                Amount = item.amount,
                Unit = item.unit,
                Ingredient = new Ingredient { Name = item.name }
            });
        }

        return recipe;
    }

    [Fact]
    public void Scale_DoublingServings_DoublesAllAmounts()
    {
        // Arrange: 4 kişilik tarif, 200gr un içeriyor
        var recipe = CreateRecipe(4, ("Un", 200, "gr"));

        // Act: 8 kişiye ölçekle
        var result = _sut.Scale(recipe, 8);

        // Assert
        Assert.Single(result);
        Assert.Equal(400, result[0].Amount);
        Assert.Equal("Un", result[0].IngredientName);
    }

    [Fact]
    public void Scale_SameServings_ReturnsOriginalAmounts()
    {
        var recipe = CreateRecipe(4, ("Şeker", 100, "gr"));

        var result = _sut.Scale(recipe, 4);

        Assert.Equal(100, result[0].Amount);
    }

    [Fact]
    public void Scale_FractionalResult_RoundsToTwoDecimals()
    {
        // 3 kişilik tarifi 2 kişiye ölçekleyince küsuratlı sonuç çıkmalı
        var recipe = CreateRecipe(3, ("Zeytinyağı", 100, "ml"));

        var result = _sut.Scale(recipe, 2);

        Assert.Equal(66.67, result[0].Amount);
    }

    [Fact]
    public void Scale_MultipleIngredients_ScalesAllProportionally()
    {
        var recipe = CreateRecipe(2,
            ("Domates", 4, "adet"),
            ("Soğan", 1, "adet"));

        var result = _sut.Scale(recipe, 6);

        Assert.Equal(12, result[0].Amount);
        Assert.Equal(3, result[1].Amount);
    }

    [Fact]
    public void Scale_ZeroTargetServings_ThrowsArgumentOutOfRangeException()
    {
        var recipe = CreateRecipe(4, ("Un", 200, "gr"));

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Scale(recipe, 0));
    }

    [Fact]
    public void Scale_NegativeTargetServings_ThrowsArgumentOutOfRangeException()
    {
        var recipe = CreateRecipe(4, ("Un", 200, "gr"));

        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Scale(recipe, -2));
    }

    [Fact]
    public void Scale_NullRecipe_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Scale(null!, 4));
    }

    [Fact]
    public void Scale_ZeroBaseServings_ThrowsInvalidOperationException()
    {
        var recipe = CreateRecipe(0, ("Un", 200, "gr"));

        Assert.Throws<InvalidOperationException>(() => _sut.Scale(recipe, 4));
    }

    [Fact]
    public void Scale_RecipeWithNoIngredients_ReturnsEmptyList()
    {
        var recipe = new Recipe { Id = 1, Title = "Boş Tarif", BaseServings = 4 };

        var result = _sut.Scale(recipe, 8);

        Assert.Empty(result);
    }
}
