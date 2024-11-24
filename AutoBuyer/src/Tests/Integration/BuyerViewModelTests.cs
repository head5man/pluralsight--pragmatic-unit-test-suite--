using AutoBuyer.Logic.Connection;
using AutoBuyer.Logic.Database;
using AutoBuyer.Logic.Domain;
using AutoBuyer.UI;
using Moq;
using Should;
using Xunit;

namespace Tests.Integration
{
    public class BuyerViewModelTests
    {
        [Fact]
        public void Joining_a_sale()
        {
            var connMock = new Mock<IStockItemConnection>();
            var repoMock = new Mock<IBuyerRepository>();
            string savedId = null;
            Buyer savedBuyer = null;
            repoMock
                .Setup(x => x.Save(It.IsAny<string>(), It.IsAny<Buyer>()))
                .Callback<string, Buyer>(
                (itemId, buyer) =>
                {
                    savedId = itemId;
                    savedBuyer = buyer;
                });

            var sut = new BuyerViewModel("1234", 1, 2, "buyer", connMock.Object, repoMock.Object);

            savedId.ShouldEqual("1234");
            savedBuyer.MaximumPrice.ShouldEqual(1);
            savedBuyer.NumberToBuy.ShouldEqual(2);
            savedBuyer.BuyerName.ShouldEqual("buyer");
            savedBuyer.Snapshot
                .VerifyState(BuyerState.Joining)
                .VerifyBoughtSoFar(0)
                .VerifyNumberInStock(0)
                .VerifyCurrentPrice(0);
        }

        [Fact]
        public void Buying_an_item()
        {
            var connMock = new Mock<IStockItemConnection>();
            var repoMock = new Mock<IBuyerRepository>();
            string savedId = null;
            Buyer savedBuyer = null;
            repoMock
                .Setup(x => x.Save(It.IsAny<string>(), It.IsAny<Buyer>()))
                .Callback<string, Buyer>(
                (itemId, buyer) =>
                {
                    savedId = itemId;
                    savedBuyer = buyer;
                });

            var sut = new BuyerViewModel("1234", 1, 2, "buyer", connMock.Object, repoMock.Object);
            connMock.Raise(x => x.MessageReceived += null, "Event: PRICE; CurrentPrice: 1; NumberInStock: 5");

            repoMock.Verify(x => x.Save(savedId, savedBuyer), Times.Exactly(2));
            savedBuyer.Snapshot
                .VerifyState(BuyerState.Buying)
                .VerifyCurrentPrice(1)
                .VerifyNumberInStock(5)
                .VerifyBoughtSoFar(0);
            connMock.Verify(x => x.SendMessage(StockCommand.Buy(1, 2)));
        }
    }
}
