namespace Core;

using Core.Domain;
using System.Collections.Generic;

public static class SampleData
{
    public static List<Product> Products() => new()
    {
        Product.Create("P-001", "SKU-001", "Цемент М400 25кг", "шт", 100),
        Product.Create("P-002", "SKU-002", "Цвяхи 100мм", "кг", 50),
        Product.Create("P-003", "SKU-003", "Пісок будівельний", "т", 10),
        Product.Create("P-004", "SKU-004", "Цегла рядова", "шт", 5), // дефіцит < 10
        Product.Create("P-005", "SKU-005", "Арматура 12мм", "м", 120),
        Product.Create("P-006", "SKU-006", "Дошка обрізна", "м", 45),
        Product.Create("P-007", "SKU-007", "Грунтовка 10л", "шт", 8),  // дефіцит < 10
        Product.Create("P-008", "SKU-008", "Фарба біла 5л", "шт", 15),
        Product.Create("P-009", "SKU-009", "Шпаклівка гіпсова", "кг", 80),
        Product.Create("P-010", "SKU-010", "Плитка керамічна", "м", 35),
        Product.Create("P-011", "SKU-011", "Пінопласт 50мм", "шт", 60),
        Product.Create("P-012", "SKU-012", "Монтажна піна", "шт", 4),  // дефіцит < 10
        Product.Create("P-013", "SKU-013", "Герметик силіконовий", "шт", 22),
        Product.Create("P-014", "SKU-014", "Щебінь фракційний", "т", 18),
        Product.Create("P-015", "SKU-015", "Лента малярна", "шт", 90)
    };
}