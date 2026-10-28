namespace Core.Abstractions;

using Core.Domain;

// Контракт, який описує, що має вміти робити будь-яке сховище 
public interface ICatalogStore
{
    IReadOnlyList<Product> List(); // Повертає список товарів (тільки для читання)
    Product? GetById(string id);   // Шукає товар за ID (може повернути null, якщо не знайдено)
    void Add(Product item);        // Додає новий запис
    void Update(Product item);     // Оновлює існуючий запис
    bool Remove(string id);        // Видаляє запис за ID

    //  пошук за умовою
    IReadOnlyList<Product> Find(Func<Product, bool> predicate);
}