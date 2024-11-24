using AutoBuyer.Logic.Connection;
using AutoBuyer.Logic.Database;
using AutoBuyer.Logic.Domain;
using AutoBuyer.UI;
using Moq;
using Should;
using Tests.Unit;
using Tests.Utils;
using Xunit;

namespace Tests.Integration
{
    public class BuyerViewModelTests : Tests
    {
        [Fact]
        public void Joining_a_sale()
        {
            var connMock = new Mock<IStockItemConnection>();

            var sut = new BuyerViewModel("1234", 1, 2, "buyer", connMock.Object, new BuyerRepository(Configuration));

            var savedBuyer = DB.GetBuyer("1234");

            savedBuyer.State.ShouldEqual(BuyerState.Joining);
            savedBuyer.MaximumPrice.ShouldEqual(1);
            savedBuyer.NumberToBuy.ShouldEqual(2);
            savedBuyer.BuyerName.ShouldEqual("buyer");
            savedBuyer.BoughtSoFar.ShouldEqual(0);
            savedBuyer.NumberInStock.ShouldEqual(0);
            savedBuyer.CurrentPrice.ShouldEqual(0);
        }

        [Fact]
        public void Buying_an_item()
        {
            var connMock = new Mock<IStockItemConnection>();

            var sut = new BuyerViewModel("1234", 1, 2, "buyer", connMock.Object, new BuyerRepository(Configuration));
            connMock.Raise(x => x.MessageReceived += null, "Event: PRICE; CurrentPrice: 1; NumberInStock: 5");

            var savedBuyer = DB.GetBuyer("1234");

            savedBuyer.State.ShouldEqual(BuyerState.Buying);
            savedBuyer.MaximumPrice.ShouldEqual(1);
            savedBuyer.NumberToBuy.ShouldEqual(2);
            savedBuyer.BuyerName.ShouldEqual("buyer");
            savedBuyer.BoughtSoFar.ShouldEqual(0);
            savedBuyer.NumberInStock.ShouldEqual(5);
            savedBuyer.CurrentPrice.ShouldEqual(1);
            connMock.Verify(x => x.SendMessage(StockCommand.Buy(1, 2)));
        }
    }
}
