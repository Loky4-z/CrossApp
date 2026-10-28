namespace Core.Storage;

using Core.Abstractions;
using System.IO;
using System.Linq;
using System;

//  Фабрика, яка вирішує, яке саме сховище створити.
// Розвантажує Program.cs від парсингу шляхів та аргументів.
public static class StoreFactory
{
    public static ICatalogStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

        // Вибираємо базове сховище (InMemory або File)
        ICatalogStore baseStore = useFile
            ? new FileCatalogStore(dataPath)
            : new InMemoryCatalogStore(SampleData.Products());

        // Обертаємо його в декоратор для кешування і повертаємо
        return new CachingCatalogStore(baseStore);
    }
}