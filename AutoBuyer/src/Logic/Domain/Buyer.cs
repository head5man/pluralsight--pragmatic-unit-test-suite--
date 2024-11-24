using System;
using System.Collections.Generic;

namespace AutoBuyer.Logic.Domain
{
    public class Buyer
    {
        public Buyer(string buyerName, int maximumPrice, int numberToBuy)
        {
            BuyerName = buyerName;
            NumberToBuy = numberToBuy;
            MaximumPrice = maximumPrice;
            Snapshot = BuyerSnapshot.Joining();
        }

        public BuyerSnapshot Snapshot { get; private set; }

        public string BuyerName { get; }

        public int MaximumPrice { get; }

        public int NumberToBuy { get; }

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

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                { nameof(BuyerName), BuyerName },
                { nameof(MaximumPrice), MaximumPrice },
                { nameof(NumberToBuy), NumberToBuy },
                { nameof(Snapshot.CurrentPrice), Snapshot.CurrentPrice },
                { nameof(Snapshot.NumberInStock), Snapshot.NumberInStock },
                { nameof(Snapshot.BoughtSoFar), Snapshot.BoughtSoFar },
                { nameof(Snapshot.State), (int)Snapshot.State }
            };
            return dict;
        }

        private StockCommand ProcessPurchaseEvent(string buyerName, int numberSold)
        {
            if (buyerName == BuyerName)
            {
                Snapshot = Snapshot.Bought(numberSold);
                if (Snapshot.BoughtSoFar >= NumberToBuy)
                {
                    Snapshot = Snapshot.Closed();
                }
            }
            return StockCommand.None();
        }

        private StockCommand ProcessPriceEvent(int currentPrice, int numberInStock)
        {
            if (currentPrice > MaximumPrice)
            {
                Snapshot = Snapshot.Monitoring(currentPrice, numberInStock);
                return StockCommand.None();
            }

            Snapshot = Snapshot.Buying(currentPrice, numberInStock);
            int numberToBuy = Math.Min(numberInStock, NumberToBuy);
            return StockCommand.Buy(currentPrice, numberToBuy);
        }

        private StockCommand ProcessCloseEvent()
        {
            Snapshot = Snapshot.Closed();
            return StockCommand.None();
        }
    }
}
