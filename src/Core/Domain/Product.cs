namespace Core.Domain;

using Core.Dto; // Підключаємо DTO з минулої лаби

public sealed class Product
{
    // 1. ІНКАПСУЛЯЦІЯ: властивості можна тільки читати (немає set).
    // Це гарантує, що ніхто ззовні не напише product.Sku = "інший_текст".
    public string Id { get; }
    public string Sku { get; }
    public string Name { get; }
    public string Unit { get; }

    // Поле _quantity приховане (private), доступ до нього тільки через методи.
    private int _quantity;
    public int Quantity => _quantity;

    // 2. ПРИВАТНИЙ КОНСТРУКТОР: 
    // Забороняє створювати об'єкт через 'new Product(...)', 
    // щоб не можна було обійти перевірки правил.
    private Product(string id, string sku, string name, string unit, int quantity)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Unit = unit;
        _quantity = quantity;
    }

    // 3. ФАБРИЧНИЙ МЕТОД: Єдина точка входу для створення товару.
    // Спочатку перевіряє ІНВАРІАНТИ (правила), і тільки якщо все ок - викликає конструктор.
    public static Product Create(string id, string sku, string name, string unit, int quantity) 
    {
        // Правило 1: SKU не порожній (проблема у вхідних даних -> ArgumentException)
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU не може бути порожнім", nameof(sku));

        // Правило 2: Початковий залишок >= 0
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Початковий залишок не може бути від'ємним");

        return new Product(id, sku, name, unit, quantity);
    }

    // 4. МЕТОДИ ЗМІНИ СТАНУ (Прихід)
    public void RegisterArrival(int amount) 
    {
        // Правило 3: Кількість для операції > 0
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Кількість має бути більшою за нуль");

        _quantity += amount; // Тільки тут фізично міняється залишок
    }

    // 5. МЕТОДИ ЗМІНИ СТАНУ (Видача)
    public void Issue(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Кількість має бути більшою за нуль");

        
        // З числом amount все ок, але ПОТОЧНИЙ СТАН не дозволяє дію -> InvalidOperationException
        if (amount > _quantity)
            throw new InvalidOperationException($"Не можна видати {amount}: залишок {Sku} = {_quantity}");

        _quantity -= amount;
    }

    // 6. МАПІНГ:
    public ProductDto ToDto() => new(Id, Sku, Name, Unit, Quantity);

    // При відновленні з DTO ми проганяємо його через той самий Create, 
    // щоб криві дані з файлу не потрапили в систему.
    public static Product FromDto(ProductDto dto) =>
        Create(dto.Id, dto.Sku, dto.Name, dto.Unit, dto.Quantity);
}