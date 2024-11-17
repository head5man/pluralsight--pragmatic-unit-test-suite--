using AutoBuyer.Logic.Connection;
using AutoBuyer.Logic.Domain;

namespace AutoBuyer.UI
{
    public class BuyerViewModel : ViewModel
    {
        public string ItemId { get; }

        private readonly Buyer _buyer;
        private readonly IStockItemConnection _connection;

        public string CurrentPrice => _buyer.Snapshot.CurrentPrice.ToString();
        public string NumberInStock => _buyer.Snapshot.NumberInStock.ToString();
        public string BoughtSoFar => _buyer.Snapshot.BoughtSoFar.ToString();
        public string State => _buyer.Snapshot.State.ToString();

        public BuyerViewModel(string itemId, int maximumPrice, int numberToBuy, string buyerName, IStockItemConnection connection)
        {
            ItemId = itemId;
            _buyer = new Buyer(buyerName, maximumPrice, numberToBuy);
            _connection = connection;
            _connection.MessageReceived += StockMessageReceived;
        }

        private void StockMessageReceived(string message)
        {
            var @event = StockEvent.From(message);
            var command = _buyer.Process(@event);
            if (command != StockCommand.None())
            {
                _connection.SendMessage(command.ToString());
            }

            Notify(string.Empty);
        }
    }
}
