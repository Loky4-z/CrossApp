using Core.Abstractions;
using Core.Storage;
using Core.Reports;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

ICatalogStore store = StoreFactory.Create(args);
var items = store.List();

var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Print(ReportService.TopByQuantity(items, 5));
        break;

    case "report-low":
        Print(ReportService.LowStock(items, 10));
        break;

    case "report-groups":
        foreach (var g in ReportService.ByUnit(items))
            Console.WriteLine($"  {g.Key,-6}  позицій: {g.Count,3}  сумарно: {g.TotalQuantity,6}");
        break;

    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($"позицій {t.Items}, всього {t.Total}, середнє {t.Average:F2}, макс {t.Max}");
        break;

    case "demo-deferred":
        RunDeferredDemo(items);
        break;

    default:
        Console.WriteLine("report-top | report-low | report-groups | report-sum | demo-deferred");
        break;
}

static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines)
        Console.WriteLine($"  {l.Sku,-10}  {l.Name,-22}  {l.Quantity,6}  {l.Unit}");
}

static void RunDeferredDemo(IEnumerable<Product> sourceItems)
{
    var source = sourceItems.ToList();
    var query = source.Where(p => p.Quantity < 10); // Запит побудовано, але ще не виконано[cite: 3]

    Console.WriteLine($"Дефіцитних до змін: {query.Count()}");

    source.Add(Product.Create("tmp00001", "SKU-999", "Скоба", "шт", 3));

    Console.WriteLine($"Дефіцитних після змін: {query.Count()}"); // Число інше, бо виконання відкладене[cite: 3, 5]

    var snapshot = query.ToList(); // Матеріалізація результату[cite: 3, 5]
    source.Clear();

    Console.WriteLine($"У знімку лишилось: {snapshot.Count}"); // Не змінюється після очищення джерела[cite: 3, 5]
}