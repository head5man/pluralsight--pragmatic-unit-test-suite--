using System;
using System.Security.Cryptography.X509Certificates;
using AutoBuyer.Logic.Domain;
using Moq;
using Should;
using Xunit;

namespace Tests
{
    public class StockEventTests
    {
        [Fact]
        public void Parses_close_event()
        {
            var stock = Parses_event_type("Event: CLOSE;", StockEventType.Close);
        }

        [Fact]
        public void Parses_price_event()
        {
            var stock = Parses_event_type("Event: PRICE; CurrentPrice: 34; NumberInStock: 12;", StockEventType.Price);
            stock.CurrentPrice.ShouldEqual(34);
            stock.NumberInStock.ShouldEqual(12);
        }
        [Fact]
        public void Parses_purchase_event()
        {
            var stock = Parses_event_type("Event: PURCHASE; BuyerName: Buyer; NumberSold: 1;", StockEventType.Purchase);
            stock.BuyerName.ShouldEqual("Buyer");
            stock.NumberSold.ShouldEqual(1);
        }

        [Fact]
        public void Does_not_parse_events_with_incorrect_format()
        {
            string message = "incorrect message";

            Action action = () => StockEvent.From(message);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Does_not_parse_events_with_unknown_types()
        {
            string message = "Event: UNKNOWN;";

            Action action = () => StockEvent.From(message);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Close_method_returns_a_close_event()
        {
            var stock = StockEvent.Close();

            stock.Type.ShouldEqual(StockEventType.Close);
            stock.ToString().ShouldEqual("Event: CLOSE;");
        }

        [Theory]
        [InlineData(10, 15)]
        public void Price_method_returns_a_price_event(int price, int inStock)
        {
            var stock = StockEvent.Price(price, inStock);

            stock.Type.ShouldEqual(StockEventType.Price);
            stock.CurrentPrice.ShouldEqual(price);
            stock.NumberInStock.ShouldEqual(inStock);
            stock.ToString().ShouldEqual($"Event: PRICE; CurrentPrice: {price}; NumberInStock: {inStock};");
        }

        [Theory]
        [InlineData("some user", 1)]
        public void Purchase_method_returns_a_purchase_event(string buyer, int numberSold)
        {
            var stock = StockEvent.Purchase(buyer, numberSold);

            stock.Type.ShouldEqual(StockEventType.Purchase);
            stock.NumberSold.ShouldEqual(numberSold);
            stock.BuyerName.ShouldEqual(buyer);
            stock.ToString().ShouldEqual($"Event: PURCHASE; BuyerName: {buyer}; NumberSold: {numberSold};");
        }

        private StockEvent Parses_event_type(string message, StockEventType type)
        {
            StockEvent stock = StockEvent.From(message);
            string serialized = stock.ToString();

            stock.Type.ShouldEqual(type);
            serialized.ShouldEqual(message);
            return stock;
        }
    }
}
