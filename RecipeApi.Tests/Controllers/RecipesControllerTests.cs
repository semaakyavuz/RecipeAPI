using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeApi.Controllers;
using RecipeApi.Data;
using RecipeApi.Models;
using RecipeApi.Services;
using Xunit;

namespace RecipeApi.Tests.Controllers;

public class RecipesControllerTests
{
    private static AppDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetById_ExistingRecipe_ReturnsOkWithRecipe()
    {
        // Arrange
        using var db = CreateInMemoryDb(nameof(GetById_ExistingRecipe_ReturnsOkWithRecipe));
        db.Recipes.Add(new Recipe { Id = 1, Title = "Mercimek Çorbası", BaseServings = 4 });
        await db.SaveChangesAsync();

        var portionScalerMock = new Mock<IPortionScalerService>();
        var nutritionMock = new Mock<INutritionInfoService>();
        var controller = new RecipesController(db, portionScalerMock.Object, nutritionMock.Object);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var recipe = Assert.IsType<Recipe>(okResult.Value);
        Assert.Equal("Mercimek Çorbası", recipe.Title);
    }

    [Fact]
    public async Task GetById_NonExistingRecipe_ReturnsNotFound()
    {
        using var db = CreateInMemoryDb(nameof(GetById_NonExistingRecipe_ReturnsNotFound));
        var portionScalerMock = new Mock<IPortionScalerService>();
        var nutritionMock = new Mock<INutritionInfoService>();
        var controller = new RecipesController(db, portionScalerMock.Object, nutritionMock.Object);

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Scale_ExistingRecipe_CallsPortionScalerAndReturnsOk()
    {
        // Arrange
        using var db = CreateInMemoryDb(nameof(Scale_ExistingRecipe_CallsPortionScalerAndReturnsOk));
        var recipe = new Recipe { Id = 1, Title = "Pilav", BaseServings = 4 };
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        var expectedResult = new List<ScaledIngredient>
        {
            new() { IngredientName = "Pirinç", Amount = 400, Unit = "gr" }
        };

        var portionScalerMock = new Mock<IPortionScalerService>();
        portionScalerMock
            .Setup(s => s.Scale(It.IsAny<Recipe>(), 8))
            .Returns(expectedResult);

        var nutritionMock = new Mock<INutritionInfoService>();
        var controller = new RecipesController(db, portionScalerMock.Object, nutritionMock.Object);

        // Act
        var result = await controller.Scale(1, 8);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var scaled = Assert.IsType<List<ScaledIngredient>>(okResult.Value);
        Assert.Single(scaled);
        Assert.Equal(400, scaled[0].Amount);

        // portionScaler'ın gerçekten çağrıldığını doğrula
        portionScalerMock.Verify(s => s.Scale(It.IsAny<Recipe>(), 8), Times.Once);
    }

    [Fact]
    public async Task Scale_InvalidTargetServings_ReturnsBadRequest()
    {
        using var db = CreateInMemoryDb(nameof(Scale_InvalidTargetServings_ReturnsBadRequest));
        db.Recipes.Add(new Recipe { Id = 1, Title = "Salata", BaseServings = 4 });
        await db.SaveChangesAsync();

        var portionScalerMock = new Mock<IPortionScalerService>();
        portionScalerMock
            .Setup(s => s.Scale(It.IsAny<Recipe>(), It.IsAny<int>()))
            .Throws(new ArgumentOutOfRangeException("targetServings", "Porsiyon sayısı 0'dan büyük olmalı."));

        var nutritionMock = new Mock<INutritionInfoService>();
        var controller = new RecipesController(db, portionScalerMock.Object, nutritionMock.Object);

        var result = await controller.Scale(1, 0);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetTotalCalories_SumsNutritionInfoAcrossIngredients()
    {
        // Arrange
        using var db = CreateInMemoryDb(nameof(GetTotalCalories_SumsNutritionInfoAcrossIngredients));
        var recipe = new Recipe { Id = 1, Title = "Omlet", BaseServings = 2 };
        recipe.RecipeIngredients.Add(new RecipeIngredient
        {
            Amount = 2, Unit = "adet", Ingredient = new Ingredient { Name = "Yumurta" }
        });
        recipe.RecipeIngredients.Add(new RecipeIngredient
        {
            Amount = 50, Unit = "gr", Ingredient = new Ingredient { Name = "Peynir" }
        });
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        var portionScalerMock = new Mock<IPortionScalerService>();
        var nutritionMock = new Mock<INutritionInfoService>();

        // İki farklı malzeme için farklı kalori değerleri dönecek şekilde mock'la
        nutritionMock
            .Setup(n => n.GetNutritionInfoAsync("Yumurta", 2, "adet"))
            .ReturnsAsync(new NutritionInfo { TotalCalories = 140, Source = "mock" });

        nutritionMock
            .Setup(n => n.GetNutritionInfoAsync("Peynir", 50, "gr"))
            .ReturnsAsync(new NutritionInfo { TotalCalories = 180, Source = "mock" });

        var controller = new RecipesController(db, portionScalerMock.Object, nutritionMock.Object);

        // Act
        var result = await controller.GetTotalCalories(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(320d, okResult.Value);

        nutritionMock.Verify(
            n => n.GetNutritionInfoAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<string>()),
            Times.Exactly(2));
    }
}
