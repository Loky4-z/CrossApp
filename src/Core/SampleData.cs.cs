namespace Core;

using Core.Domain;

public static class SampleData
{
    public static List<Product> Products() => new()
    {
        Product.Create("P-001", "SKU-001", "Цемент M400 25кг", "шт", 100),
        Product.Create("P-002", "SKU-002", "Цвяхи 100мм", "кг", 50),
        Product.Create("P-003", "SKU-003", "Пісок будівельний", "т", 10)
    };
}