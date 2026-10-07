namespace Core.Domain;

public enum OrderStatus { Draft, Confirmed, Cancelled }

public sealed class Order
{
    public string Id { get; }
    public OrderStatus Status { get; private set; }

    public Order(string id)
    {
        Id = id;
        Status = OrderStatus.Draft;
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        Status = (Status, newStatus) switch
        {
            (OrderStatus.Draft, OrderStatus.Confirmed) => newStatus,
            (OrderStatus.Draft, OrderStatus.Cancelled) => newStatus,
            _ => throw new InvalidOperationException($"Неможливий перехід статусу з {Status} в {newStatus}")
        };
    }
}