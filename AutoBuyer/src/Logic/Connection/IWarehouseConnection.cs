namespace AutoBuyer.Logic.Connection
{
    public interface IWarehouseConnection
    {
        IStockItemConnection ConnectToItem(string itemId, string buyerName);
    }
}
