using AutoBuyer.Logic.Domain;

namespace AutoBuyer.Logic
{
    public interface IPortfolioListener
    {
        void BuyerAdded(Buyer buyer);
    }
}
