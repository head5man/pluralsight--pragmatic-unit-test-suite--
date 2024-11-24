using AutoBuyer.Logic.Connection;
using AutoBuyer.Logic.Database;
using AutoBuyer.Logic.Domain;

namespace AutoBuyer.UI
{
    public class BuyerViewModel : ViewModel
    {
        public string ItemId { get; }

        private readonly Buyer _buyer;
        private readonly IStockItemConnection _connection;
        private readonly BuyerRepository _repository;

        public string CurrentPrice => _buyer.Snapshot.CurrentPrice.ToString();
        public string NumberInStock => _buyer.Snapshot.NumberInStock.ToString();
        public string BoughtSoFar => _buyer.Snapshot.BoughtSoFar.ToString();
        public string State => _buyer.Snapshot.State.ToString();

        public BuyerViewModel(
            string itemId,
            int maximumPrice,
            int numberToBuy,
            string buyerName,
            IStockItemConnection connection,
            BuyerRepository repository)
        {
            ItemId = itemId;
            _buyer = new Buyer(buyerName, maximumPrice, numberToBuy);
            _connection = connection;
            _repository = repository;
            _connection.MessageReceived += StockMessageReceived;
            _repository.Save(ItemId, _buyer);
        }

        private void StockMessageReceived(string message)
        {
            var @event = StockEvent.From(message);
            var command = _buyer.Process(@event);
            if (command != StockCommand.None())
            {
                _connection.SendMessage(command.ToString());
            }

            _repository.Save(ItemId, _buyer);

            Notify(nameof(CurrentPrice));
            Notify(nameof(NumberInStock));
            Notify(nameof(BoughtSoFar));
            Notify(nameof(State));
        }
    }
}
