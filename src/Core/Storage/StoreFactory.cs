namespace Core.Storage;

using Core.Abstractions;
using System.IO;
using System.Linq;
using System;

public static class StoreFactory
{
    public static ICatalogStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

        ICatalogStore baseStore = useFile
            ? new FileCatalogStore(dataPath)
            : new InMemoryCatalogStore(SampleData.Products());

        // Обертаємо базове сховище в наш декоратор
        return new CachingCatalogStore(baseStore);
    }
}