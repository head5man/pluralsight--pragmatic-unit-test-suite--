using AutoBuyer.Logic;
using AutoBuyer.Logic.Domain;
using Should;
using System;
using Xunit;

namespace Tests
{
    public class BuyerTests
    {
        [Fact]
        public void New_buyer_is_in_joining_state()
        {
            var buyer = CreateJoiningBuyer();

            buyer.Snapshot.VerifyState(BuyerState.Joining);
        }

        [Fact]
        public void Closes_when_item_closes()
        {
            var buyer = CreateJoiningBuyer();
            
            StockCommand command = buyer.Process(StockEvent.Close());

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot
                .VerifyState(BuyerState.Closed)
                .VerifyCurrentPrice(0)
                .VerifyBoughtSoFar(0)
                .VerifyNumberInStock(0)
                .VerifyItemId("id");
        }

        [Fact]
        public void Buyer_does_not_buy_when_price_event_with_too_high_price_arrives()
        {
            var buyer = CreateJoiningBuyer(maximumPrice: 10);

            StockCommand command = buyer.Process(StockEvent.Price(20, 5));
            
            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot
                .VerifyState(BuyerState.Monitoring)
                .VerifyCurrentPrice(20)
                .VerifyBoughtSoFar(0)
                .VerifyNumberInStock(5)
                .VerifyItemId("id");
        }

        [Fact]
        public void Buyer_buys_when_price_event_with_appropriate_price_arrives()
        {
            var buyer = CreateJoiningBuyer(50);

            StockCommand command = buyer.Process(StockEvent.Price(10, 5));

            command.ShouldEqual(StockCommand.Buy(10, 1));
            buyer.Snapshot
                .VerifyState(BuyerState.Buying)
                .VerifyCurrentPrice(10)
                .VerifyBoughtSoFar(0)
                .VerifyNumberInStock(5);
        }

        [Fact]
        public void Buyer_attempts_to_buy_maximum_amount_available()
        {
            var buyer = CreateJoiningBuyer(50, 10);

            StockCommand command = buyer.Process(StockEvent.Price(10, 5));

            command.ShouldEqual(StockCommand.Buy(10, 5));
            buyer.Snapshot
                .VerifyState(BuyerState.Buying)
                .VerifyCurrentPrice(10)
                .VerifyBoughtSoFar(0)
                .VerifyNumberInStock(5);
        }

        [Fact]
        public void Buyer_does_not_react_to_a_purchase_event_related_to_another_buyer()
        {
            Buyer sut = CreateMonitoringBuyer("Buyer");

            StockCommand command = sut.Process(StockEvent.Purchase("Some other buyer", 1));

            command.ShouldEqual(StockCommand.None());
            sut.Snapshot.State.ShouldEqual(BuyerState.Monitoring);
            sut.Snapshot.BoughtSoFar.ShouldEqual(0);
        }

        [Fact]
        public void Buyer_updates_items_bought_so_far_when_purchase_event_with_the_same_user_name_arrives()
        {
            var numberInStock = 10;
            var numberToBuy = 1;
            var buyer = CreateMonitoringBuyer("name", numberInStock: numberInStock);

            StockCommand command = buyer.Process(StockEvent.Purchase("name", numberToBuy));

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot.State.ShouldEqual(BuyerState.Monitoring);
            buyer.Snapshot.BoughtSoFar.ShouldEqual(numberToBuy);
            buyer.Snapshot.NumberInStock.ShouldEqual(numberInStock - numberToBuy);
        }

        [Fact]
        public void Buyer_closes_when_it_buys_enough_items()
        {
            var buyer = CreateMonitoringBuyer("Buyer", numberToBuy: 5);

            StockCommand command = buyer.Process(StockEvent.Purchase("Buyer", 5));

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot.State.ShouldEqual(BuyerState.Closed);
        }

        [Fact]
        public void Closed_buyer_does_not_react_to_further_messages_()
        {
            var maximumPrice = 5;
            var buyer = CreateClosed(maximumPrice);

            StockCommand command = buyer.Process(StockEvent.Price(maximumPrice, 10));

            command.ShouldEqual(StockCommand.None());
            buyer.Snapshot.State.ShouldEqual(BuyerState.Closed);
        }

        [Fact]
        public void Buyer_thows_exception_for_unknown_event()
        {
            var buyer = CreateJoiningBuyer();

            var action = (Action)( () => buyer.Process(StockEvent.None()) );

            Assert.Throws<InvalidOperationException>(action);
        }

        private Buyer CreateClosed(int maximumPrice = 5)
        {
            var buyer = CreateJoiningBuyer(maximumPrice, 1, "Buyer");
            buyer.Process(StockEvent.Close());
            return buyer;
        }

        private Buyer CreateMonitoringBuyer(string buyer, int numberInStock = 100, int numberToBuy = 10)
        {
            var ret = CreateJoiningBuyer(maximumPrice: 100, numberToBuy: numberToBuy, name: buyer);
            ret.Process(StockEvent.Price(200, numberInStock));
            return ret;
        }

        private Buyer CreateJoiningBuyer(int maximumPrice = 100, int numberToBuy = 1, string name = "", string itemId = "id")
        {
            return new Buyer(buyerName: name, itemId: itemId, maximumPrice: maximumPrice, numberToBuy: numberToBuy, stockItem: null);
        }
    }

    internal static class BuyerExtensions
    {
        public static BuyerSnapshot VerifyState(this BuyerSnapshot snap, BuyerState state)
        {
            snap.State.ShouldEqual(state);
            return snap;
        }

        public static BuyerSnapshot VerifyCurrentPrice(this BuyerSnapshot snap, int currentPrice)
        {
            snap.CurrentPrice.ShouldEqual(currentPrice);
            return snap;
        }

        public static BuyerSnapshot VerifyBoughtSoFar(this BuyerSnapshot snap, int boughtSoFar)
        {
            snap.BoughtSoFar.ShouldEqual(boughtSoFar);
            return snap;
        }


        public static BuyerSnapshot VerifyNumberInStock(this BuyerSnapshot snap, int numberInStock)
        {
            snap.NumberInStock.ShouldEqual(numberInStock);
            return snap;
        }


        public static BuyerSnapshot VerifyItemId(this BuyerSnapshot snap, string id)
        {
            snap.ItemId.ShouldEqual(id);
            return snap;
        }
    }
}
