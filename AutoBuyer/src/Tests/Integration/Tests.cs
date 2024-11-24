using AutoBuyer.Common;
using AutoBuyer.UI;
using MySqlConnector;
using MySqlX.Serialization;
using System.Collections.Generic;
using System.Reflection;

namespace Tests.Integration
{
    public class Tests
    {
        public static IConfig Configuration { get; set; }

        public Tests()
        {
            Configure();
            ClearDatabase();
        }

        private void Configure()
        {
            if (Configuration is null)
            {
                Configuration = ConfigBuilder.Build();
            }
        }

        private void ClearDatabase()
        {
            string query = @"DELETE FROM `Buyer`";

            using (var connection = new MySqlConnection(Configuration.Connection))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        private static class ConfigBuilder
        {
            public static IConfig Build()
            {
                return Config.Build(new Parser());
            }

            private class Parser : IParser
            {
                public Dictionary<string, object> Parse(string input) => JsonParser.Parse(input);
            }
        }

    }
}
