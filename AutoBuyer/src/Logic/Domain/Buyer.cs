using System;

namespace AutoBuyer.Logic.Domain
{
    public class Buyer
    {
        private readonly string _buyerName;
        private readonly int _maximumPrice;
        private readonly int _numberToBuy;
        public BuyerSnapshot Snapshot { get; private set; }

        public Buyer(string buyerName, int maximumPrice, int numberToBuy)
        {
            _buyerName = buyerName;
            _numberToBuy = numberToBuy;
            _maximumPrice = maximumPrice;
            Snapshot = BuyerSnapshot.Joining();
        }

        public StockCommand Process(StockEvent @event)
        {
            if (Snapshot.State != BuyerState.Closed)
            {
                switch (@event.Type)
                {
                    case StockEventType.Price:
                        return ProcessPriceEvent(@event.CurrentPrice, @event.NumberInStock);

                    case StockEventType.Purchase:
                        return ProcessPurchaseEvent(@event.BuyerName, @event.NumberSold);

                    case StockEventType.Close:
                        return ProcessCloseEvent();

                    default:
                        throw new InvalidOperationException();
                }
            }
            return StockCommand.None();
        }

        private StockCommand ProcessPurchaseEvent(string buyerName, int numberSold)
        {
            if (buyerName == _buyerName)
            {
                Snapshot = Snapshot.Bought(numberSold);
                if (Snapshot.BoughtSoFar >= _numberToBuy)
                {
                    Snapshot = Snapshot.Closed();
                }
            }
            return StockCommand.None();
        }

        private StockCommand ProcessPriceEvent(int currentPrice, int numberInStock)
        {
            if (currentPrice > _maximumPrice)
            {
                Snapshot = Snapshot.Monitoring(currentPrice, numberInStock);
                return StockCommand.None();
            }

            Snapshot = Snapshot.Buying(currentPrice, numberInStock);
            int numberToBuy = Math.Min(numberInStock, _numberToBuy);
            return StockCommand.Buy(currentPrice, numberToBuy);
        }

        private StockCommand ProcessCloseEvent()
        {
            Snapshot = Snapshot.Closed();
            return StockCommand.None();
        }
    }
}
