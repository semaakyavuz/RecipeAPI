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

## CI/CD + SonarCloud kurulumu

1. [sonarcloud.io](https://sonarcloud.io) üzerinde GitHub hesabınla giriş yap, bu repoyu "import" et.
2. Sonar'ın sana verdiği **project key** ve **organization key**'i `.github/workflows/ci.yml` içindeki
   `CHANGE_ME_sonar-project-key` ve `CHANGE_ME_sonar-org` yerlerine yaz.
3. SonarCloud'da bir **token** üret (My Account → Security → Generate Token).
4. GitHub repo → Settings → Secrets and variables → Actions → New repository secret:
   - Name: `SONAR_TOKEN`
   - Value: (ürettiğin token)
5. `main` branch'ine push/PR attığında Actions sekmesinde workflow otomatik çalışır; SonarCloud dashboard'unda coverage ve code smell raporlarını görürsün.

## Neden bu proje?

Bu proje, StockBridge'de uygulanan xUnit + Moq + GitHub Actions CI + SonarCloud kurulumunu farklı ve daha küçük bir domain üzerinde tekrar ederek pekiştirmek amacıyla sıfırdan yazıldı.
