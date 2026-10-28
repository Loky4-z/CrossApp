namespace Core.Services;

using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CatalogService
{
    private readonly ICatalogStore _store;
    private readonly INotificationSink? _notificationSink;

    // Конструктор тепер може приймати і сховище, і синк сповіщень
    public CatalogService(ICatalogStore store, INotificationSink? notificationSink = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _notificationSink = notificationSink;
    }

    public event EventHandler<CatalogChangedEventArgs>? Changed;

    private void OnChanged(Product p, CatalogChangeKind kind, int delta)
    {
        var args = new CatalogChangedEventArgs(p.Id, kind, delta, p.Quantity);

        // 1. Стандартна подія
        Changed?.Invoke(this, args);

        // 2. Додатковий бонусний Sink (якщо він переданий)
        if (_notificationSink != null)
        {
            var line = $"{args.At:HH:mm:ss} {args.Kind,-8} {args.Id} {args.Delta:+0;-0} залишок {args.QuantityAfter}";
            _notificationSink.Send(line);
        }
    }

    public Product Add(string sku, string name, string unit, int quantity)
    {
        var product = Product.Create(Guid.NewGuid().ToString("N")[..8], sku, name, unit, quantity);
        _store.Add(product);
        OnChanged(product, CatalogChangeKind.Added, quantity);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        product.RegisterArrival(quantity);
        _store.Update(product);
        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        product.Issue(quantity);
        _store.Update(product);
        OnChanged(product, CatalogChangeKind.Issued, quantity);
    }

    public void Remove(string id)
    {
        var product = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        _store.Remove(id);
        OnChanged(product, CatalogChangeKind.Removed, -product.Quantity);
    }

    public IReadOnlyList<Product> All() => _store.List();
    public Product? Find(string id) => _store.GetById(id);
    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate) => _store.List().Where(predicate).ToList();
}