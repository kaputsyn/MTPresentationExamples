
namespace Courier.Api;
public class SubmitOrderRequest
{
    public string CustomerName { get; set; }
    public string CustomerCard { get; set; }

    public string Address { get; set; }

    public List<OrderItem> Items { get; set; }
}


public class OrderItem
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
}
