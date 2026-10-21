using Core;
using Core.Abstractions;
using Core.Storage;
using Core.Services;
using System;

// Створюємо сховище через фабрику
ICatalogStore store = StoreFactory.Create(args);
var service = new CatalogService(store);

Console.WriteLine($"Сховище налаштовано: {store.GetType().Name}");

// Успішний сценарій
var created = service.Add("SKU-101", "Шурупокрут Bosch", "шт", 15);
service.Receive(created.Id, 5);
service.Issue(created.Id, 2);

Console.WriteLine("\n--- Поточний стан складу ---");
foreach (var p in service.All())
{
    Console.WriteLine($"{p.Id} {p.Sku,-10} {p.Name,-22} {p.Quantity,5} {p.Unit}");
}

// Сценарій відмови
Console.WriteLine("\n--- Сценарій відмови ---");
try
{
    service.Issue("INVALID-ID", 10);
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}