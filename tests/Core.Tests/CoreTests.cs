using Core;
using Core.Domain;
using Core.Events;
using Core.Reports;
using Core.Services;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests;

public static class TestData
{
    public static List<Product> Items() => new()
{
    Product.Create("1", "SKU-BIG", "Цемент", "шт", 100),
    Product.Create("2", "SKU-MID", "Пісок", "кг", 50),
    Product.Create("3", "SKU-LOW", "Цвяхи", "кг", 5)
};
}

public class CoreTests
{
    [Fact] // 1) Інваріант: відмова і незмінний стан[cite: 16]
    public void Receive_WithNegativeQuantity_Throws_AndKeepsState()
    {
        var store = new InMemoryCatalogStore();
        var service = new CatalogService(store);
        var p = service.Add("SKU-1", "Кабель", "м", 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.Receive(p.Id, -5));
        Assert.Equal(10, store.GetById(p.Id)!.Quantity);
    }

    [Fact] // 2) Подія як спосіб перевірки[cite: 16]
    public void Add_RaisesChangedExactlyOnce()
    {
        var service = new CatalogService(new InMemoryCatalogStore());
        var events = new List<CatalogChangedEventArgs>();
        service.Changed += (_, e) => events.Add(e);

        service.Add("SKU-2", "Гвинт", "шт", 7);

        Assert.Single(events);
        Assert.Equal(CatalogChangeKind.Added, events[0].Kind);
    }

    [Theory] // 3) Звіт на кількох наборах[cite: 16]
    [InlineData(10, 1)]
    [InlineData(60, 2)]
    public void LowStock_CountsOnlyBelowThreshold(int threshold, int expected)
    {
        Assert.Equal(expected, ReportService.LowStock(TestData.Items(), threshold).Count);
    }

    [Fact] // 4) Детермінований топ-N[cite: 17]
    public void TopByQuantity_IsDeterministic()
    {
        var result = ReportService.TopByQuantity(TestData.Items(), 2).Select(i => i.Sku);
        Assert.Equal(new[] { "SKU-BIG", "SKU-MID" }, result);
    }

    [Fact] // 5) Скасування[cite: 17]
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var service = new CatalogService(new InMemoryCatalogStore());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.ImportAsync(new JsonCatalogImporter(), "any.json", cts.Token));
    }

    [Fact] // 6) Некоректні дані у TempPath -> DomainException[cite: 17]
    public async Task ImportAsync_InvalidData_ThrowsDomainException()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        var json = "[{\"Sku\":\"SKU-ERR\",\"Name\":\"Помилка\",\"Unit\":\"шт\",\"Quantity\":-10}]";
        await File.WriteAllTextAsync(tempFile, json);

        try
        {
            var service = new CatalogService(new InMemoryCatalogStore());
            var ex = await Assert.ThrowsAsync<DomainException>(
                () => service.ImportAsync(new JsonCatalogImporter(), tempFile));

            Assert.Contains("кількість не може бути 0", ex.Message);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}