using Core;
using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

string extension = Path.GetExtension(path).ToLowerInvariant();

// Вибір імпортера на основі розширення файлу
var result = extension switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    _ => throw new NotSupportedException($"Формат файлу {extension} не підтримується")
};

// Вивід товарів
Console.WriteLine($"Завантажено товарів: {result.Products.Count}");
foreach (var p in result.Products)
    Console.WriteLine($" [Товар] {p.Id,-6} {p.Name,-26} {p.Quantity,5} {p.Unit}");

// Вивід складів
Console.WriteLine($"\nЗавантажено складів: {result.Warehouses.Count}");
foreach (var w in result.Warehouses)
    Console.WriteLine($" [Склад] {w.Id,-6} {w.Name,-15} {w.Location}");

// Вивід помилок
if (result.Errors.Count > 0)
{
    Console.WriteLine($"\nПропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
        Console.WriteLine($" ! {e}");
}

// Статистика
int totalAccepted = result.Products.Count + result.Warehouses.Count;
int total = totalAccepted + result.Errors.Count;
double errorPercent = total == 0 ? 0 : (double)result.Errors.Count / total * 100;

Console.WriteLine($"\n---");
Console.WriteLine($"СТАТИСТИКА: усього {total} / прийнято {totalAccepted} / пропущено {result.Errors.Count} / помилок {errorPercent:F1}%");

return 0;