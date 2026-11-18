using System.Reflection;
using Core.Configuration;
using Core.Services;
using Core.Storage;
using Microsoft.Extensions.Configuration;

var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .AddCommandLine(args)
    .Build();

var options = config.Get<AppOptions>() ?? new AppOptions();
var legacy = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");

var storageOptions = new StorageOptions
{
    DataPath = string.IsNullOrWhiteSpace(legacy) ? options.Storage.DataPath : legacy,
    FileName = options.Storage.FileName
};

var fullPath = storageOptions.ResolveFullPath(AppContext.BaseDirectory);
var apiKey = config["Integration:ApiKey"];

Console.WriteLine($"Файл даних: {fullPath}");
Console.WriteLine(apiKey is null ? "ApiKey: не заданий" : $"ApiKey: присутній, довжина {apiKey.Length}");

// Ініціалізація сховища та сервісу
var store = new FileCatalogStore(fullPath);
var catalogService = new CatalogService(store);

if (args.Length == 0)
{
    Console.WriteLine("Використання: dotnet run --project src/Cli -- [ команда ]");
    Console.WriteLine("Доступні команди: list, export тощо.");
    return;
}

var command = args[0].ToLowerInvariant();

switch (command)
{
    case "list":
        var items = catalogService.All();
        Console.WriteLine($"Знайдено товарів: {items.Count}");
        foreach (var item in items)
        {
            Console.WriteLine($"- [{item.Sku}] {item.Name} ({item.Quantity} {item.Unit})");
        }
        break;

    case "export":
        var allItems = catalogService.All();
        Console.WriteLine($"Експортовано {allItems.Count} записи.");
        break;

    default:
        Console.WriteLine($"Невідома команда: {command}");
        break;
}