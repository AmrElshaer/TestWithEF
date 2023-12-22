namespace TestWithEF.Entities;

public class OrderProduct
{
    private OrderProduct() { }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public static OrderProduct Create(Guid productId, int quantity)
    {
        return new OrderProduct()
        {
            ProductId = productId,
            Quantity = quantity,
        };
    }
}
