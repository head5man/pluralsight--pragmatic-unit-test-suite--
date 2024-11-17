using System;

namespace AutoBuyer.Logic.Connection
{
    public interface IStockItemConnection
    {
        event Action<string> MessageReceived;
        string BuyerName { get; }
        void SendMessage(string message);
    }
}
