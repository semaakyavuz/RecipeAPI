using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Data;
using RecipeApi.Models;
using RecipeApi.Services;

namespace RecipeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPortionScalerService _portionScaler;
    private readonly INutritionInfoService _nutritionInfo;

    public RecipesController(
        AppDbContext db,
        IPortionScalerService portionScaler,
        INutritionInfoService nutritionInfo)
    {
        _db = db;
        _portionScaler = portionScaler;
        _nutritionInfo = nutritionInfo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
    {
        var recipes = await _db.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .ToListAsync();

        return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetById(int id)
    {
        var recipe = await _db.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
        {
            return NotFound();
        }

        return Ok(recipe);
    }

    [HttpPost]
    public async Task<ActionResult<Recipe>> Create(Recipe recipe)
    {
        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
    }

    [HttpGet("{id}/scale/{targetServings}")]
    public async Task<ActionResult<List<ScaledIngredient>>> Scale(int id, int targetServings)
    {
        var recipe = await _db.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
        {
            return NotFound();
        }

        try
        {
            var scaled = _portionScaler.Scale(recipe, targetServings);
            return Ok(scaled);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/nutrition")]
    public async Task<ActionResult<double>> GetTotalCalories(int id)
    {
        var recipe = await _db.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
        {
            return NotFound();
        }

        double total = 0;
        foreach (var ri in recipe.RecipeIngredients)
        {
            var info = await _nutritionInfo.GetNutritionInfoAsync(
                ri.Ingredient?.Name ?? string.Empty, ri.Amount, ri.Unit);
            total += info.TotalCalories;
        }

        return Ok(total);
    }
}
