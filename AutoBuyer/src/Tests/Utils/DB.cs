using AutoBuyer.Logic.Domain;
using Dapper;
using MySqlConnector;
using System.Linq;

namespace Tests.Utils
{
    public static class DB
    {
        public static BuyerDto GetBuyer(string itemId)
        {
            using (var connection = new MySqlConnection(Integration.Tests.Configuration.Connection))
            {
                string query = "SELECT * FROM `Buyer` WHERE ItemID = @ItemId";
                return connection.Query<BuyerDto>(query, new { ItemID = itemId }).Single();
            }
        }

        public struct BuyerDto
        {
            public readonly string BuyerName;
            public readonly int MaximumPrice;
            public readonly int NumberToBuy;
            public readonly int CurrentPrice;
            public readonly int NumberInStock;
            public readonly int BoughtSoFar;
            public readonly BuyerState State;
        }
    }
}
