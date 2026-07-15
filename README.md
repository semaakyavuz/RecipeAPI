# RecipeApi

Basit bir yemek tarifi (Recipe) yönetim API'si — ASP.NET Core 8 Web API, EF Core (SQLite), xUnit + Moq test altyapısı ve GitHub Actions CI/CD + SonarCloud entegrasyonu.

## Domain

- **Recipe**: Tarif (başlık, talimatlar, baz porsiyon sayısı)
- **Ingredient**: Malzeme
- **RecipeIngredient**: Tarif-malzeme ilişkisi (miktar, birim)

## Test edilebilir iş mantığı

- **PortionScalerService**: Bir tarifin malzeme miktarlarını istenen porsiyon sayısına göre ölçekler. Saf hesaplama mantığı olduğu için Moq gerektirmeyen, doğrudan xUnit ile test edilen servis (`PortionScalerServiceTests.cs`).
- **INutritionInfoService**: Dış bir besin bilgisi API'sini simüle eder (gerçek projede Edamam/Nutritionix gibi bir servise HTTP isteği atar). Controller testlerinde **Moq** ile mock'lanır (`RecipesControllerTests.cs`).

## Yerelde çalıştırma

```bash
dotnet restore RecipeApi.sln
dotnet build RecipeApi.sln
dotnet run --project RecipeApi.csproj
```

Swagger arayüzü: `https://localhost:{port}/swagger`

## Testleri çalıştırma

```bash
dotnet test RecipeApi.Tests/RecipeApi.Tests.csproj
```

Coverage ile birlikte:

```bash
dotnet test RecipeApi.Tests/RecipeApi.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --settings coverlet.runsettings \
  --results-directory ./coverage
```
