using System;
using System.Collections.Generic;

namespace AutoBuyer.Logic.Domain
{
    public class Buyer : IStockEventListener
    {
        private readonly List<IBuyerListener> _listeners = new List<IBuyerListener>();
        private readonly string _buyerName;
        private readonly int _maximumPrice;
        private readonly int _numberToBuy;
        private readonly IStockItem _stockItem;
        public BuyerSnapshot Snapshot { get; private set; }

        public Buyer(string buyerName, string itemId, int maximumPrice, int numberToBuy, IStockItem stockItem)
        {
            _buyerName = buyerName;
            _numberToBuy = numberToBuy;
            _maximumPrice = maximumPrice;
            _stockItem = stockItem;
            Snapshot = BuyerSnapshot.Joining(itemId);
        }

        public void AddBuyerListener(IBuyerListener listener)
        {
            _listeners.Add(listener);
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

        public void CurrentPrice(int price, int numberInStock)
        {
            if (Snapshot.State == BuyerState.Closed)
                return;

            if (price > _maximumPrice)
            {
                Snapshot = Snapshot.Monitoring(price, numberInStock);
            }
            else
            {
                int numberToBuy = Math.Min(numberInStock, _numberToBuy);
                _stockItem.Buy(price, numberToBuy);
                Snapshot = Snapshot.Buying(price, numberInStock);
            }
            NotifyChange();
        }

        public void ItemPurchased(int numberSold, PurchaseSource purchaseSource)
        {
            if (purchaseSource == PurchaseSource.FromBuyer)
            {
                Snapshot = Snapshot.Bought(numberSold);
                if (Snapshot.BoughtSoFar >= _numberToBuy)
                {
                    Snapshot = Snapshot.Closed();
                }
                NotifyChange();
            }
        }

        public void ItemClosed()
        {
            Snapshot = Snapshot.Closed();
            NotifyChange();
        }

        private void NotifyChange()
        {
            foreach (IBuyerListener listener in _listeners)
            {
                listener.BuyerStateChanged(Snapshot);
            }
        }
    }
}
