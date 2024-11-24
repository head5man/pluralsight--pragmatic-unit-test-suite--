using AutoBuyer.Logic.Domain;

namespace AutoBuyer.Logic.Database
{
    public interface IBuyerRepository
    {
        void Save(string itemId, Buyer buyer);
    }
}