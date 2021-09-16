namespace Courier.Components.Purchase
{
    public interface PurchaseActivityArguments
    {
        decimal Sum { get; }

        string CustomerCard { get; }

        string CustomerName {  get; }
    }

    
}