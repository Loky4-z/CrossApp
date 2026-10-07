namespace Core.Domain;

using Core.Dto;
using Core.Import;

public static class ImportValidator
{
    public static (IReadOnlyList<Product> ValidProducts, IReadOnlyList<string> Errors) Validate(CatalogImportResult result)
    {
        var products = new List<Product>();
        var errors = new List<string>(result.Errors); // Беремо помилки з файлу (з 3 лаби)

        foreach (var dto in result.Products)
        {
            try
            {
                // Тут спрацюють наші перевірки (інваріанти)
                products.Add(Product.FromDto(dto));
            }
            catch (Exception ex)
            {
                errors.Add($"Помилка правил для {dto.Id}: {ex.Message}");
            }
        }
        return (products.AsReadOnly(), errors.AsReadOnly());
    }
}