namespace Core.Import;

using System.Text.Json;
using Core.Dto;

public static class ProductJsonImporter
{
    public static CatalogImportResult Load(string path)
    {
        string json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var items = JsonSerializer.Deserialize<List<ProductDto>>(json, options) ?? [];

        return new CatalogImportResult(items, new List<WarehouseDto>(), new List<string>());
    }
}