namespace Core.Import;

using Core.Dto;

public record CatalogImportResult(
    IReadOnlyList<ProductDto> Products,
    IReadOnlyList<WarehouseDto> Warehouses,
    IReadOnlyList<string> Errors
);

public static class ProductCsvImporter
{
    private const char Separator = ';';

    public static CatalogImportResult Load(string path)
    {
        var products = new List<ProductDto>();
        var warehouses = new List<WarehouseDto>();
        var errors = new List<string>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase)) continue;

            switch (ParseLine(line))
            {
                case ParseProductOk p: products.Add(p.Value); break;
                case ParseWarehouseOk w: warehouses.Add(w.Value); break;
                case ParseFailed failed: errors.Add($"рядок {number}: {failed.Reason}"); break;
            }
        }
        return new CatalogImportResult(products, warehouses, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            // --- Патерни для Товарів (починаються на P) ---
            [var id, "", _, _, _] when id.StartsWith("P", StringComparison.OrdinalIgnoreCase)
                => new ParseFailed("SKU порожній"),
            [var id, _, "", _, _] when id.StartsWith("P", StringComparison.OrdinalIgnoreCase)
                => new ParseFailed("Назва порожня"),
            [var id, _, _, _, var qty] when id.StartsWith("P", StringComparison.OrdinalIgnoreCase) && (!int.TryParse(qty, out int q) || q < 0)
                => new ParseFailed($"кількість '{qty}' не є невід'ємним числом"),
            [var id, var sku, var name, var unit, var qty] when id.StartsWith("P", StringComparison.OrdinalIgnoreCase)
                => new ParseProductOk(new ProductDto(id, sku, name, unit, int.Parse(qty))),

            // --- Патерни для Складів (починаються на W) ---
            [var id, var name, var loc] when id.StartsWith("W", StringComparison.OrdinalIgnoreCase)
                => new ParseWarehouseOk(new WarehouseDto(id, name, loc)),
            [var id, ..] when id.StartsWith("W", StringComparison.OrdinalIgnoreCase)
                => new ParseFailed($"неправильний формат складу, очікую 3 колонки, отримав {parts.Length}"),

            // --- Усе інше ---
            _ => new ParseFailed("невідомий формат або неправильна кількість колонок")
        };
    }
}

abstract record ParseOutcome;
sealed record ParseProductOk(ProductDto Value) : ParseOutcome;
sealed record ParseWarehouseOk(WarehouseDto Value) : ParseOutcome;
sealed record ParseFailed(string Reason) : ParseOutcome;