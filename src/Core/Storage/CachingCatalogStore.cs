namespace Core.Storage;

using Core.Abstractions;
using Core.Domain;

public class CachingCatalogStore : ICatalogStore
{
    private readonly ICatalogStore _inner;
    private readonly Dictionary<string, Product> _cache = new();

    public CachingCatalogStore(ICatalogStore inner)
    {
        _inner = inner;
    }

    public IReadOnlyList<Product> List() => _inner.List();

    public Product? GetById(string id)
    {
        // Якщо є в кеші — віддаємо з кешу  
        if (_cache.TryGetValue(id, out var cached)) return cached;

        // Якщо ні — йдемо в реальне сховище і кешуємо результат
        var product = _inner.GetById(id);
        if (product != null) _cache[id] = product;
        return product;
    }

    public void Add(Product item)
    {
        _inner.Add(item);
        _cache[item.Id] = item;
    }

    public void Update(Product item)
    {
        _inner.Update(item);
        _cache[item.Id] = item;
    }

    public bool Remove(string id)
    {
        if (_inner.Remove(id))
        {
            _cache.Remove(id);
            return true;
        }
        return false;
    }

    public IReadOnlyList<Product> Find(Func<Product, bool> predicate) => _inner.Find(predicate);
}