using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileCatalogStore : ICatalogStore
{
    private readonly string _path;
    private readonly Dictionary<string, Product> _cache = new(StringComparer.OrdinalIgnoreCase);
    private bool _loaded;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public FileCatalogStore(string fullPath)
    {
        _path = fullPath;
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;
        if (!File.Exists(_path)) { _loaded = true; return; }

        var json = File.ReadAllText(_path);
        var dtos = JsonSerializer.Deserialize<List<ProductDto>>(json, Options) ?? [];

        _cache.Clear();
        foreach (var dto in dtos)
        {
            var product = Product.FromDto(dto);
            _cache[product.Id] = product;
        }
        _loaded = true;
    }

    private void Flush()
    {
        EnsureLoaded();
        var dtos = _cache.Values.Select(product => product.ToDto()).ToList();
        var tmp = _path + ".tmp";

        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(tmp, JsonSerializer.Serialize(dtos, Options));
        File.Move(tmp, _path, overwrite: true);
    }

    public IReadOnlyList<Product> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public Product? GetById(string id)
    {
        EnsureLoaded();
        return _cache.TryGetValue(id, out var p) ? p : null;
    }

    public IReadOnlyList<Product> Find(Func<Product, bool> predicate)
    {
        EnsureLoaded();
        return _cache.Values.Where(predicate).ToList();
    }

    public void Add(Product product)
    {
        EnsureLoaded();
        _cache[product.Id] = product;
        Flush();
    }

    public void Update(Product product)
    {
        EnsureLoaded();
        _cache[product.Id] = product;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        if (_cache.Remove(id))
        {
            Flush();
            return true;
        }
        return false;
    }
}