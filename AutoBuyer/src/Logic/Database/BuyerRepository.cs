using AutoBuyer.Logic.Common;
using AutoBuyer.Logic.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace AutoBuyer.Logic.Database
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly IConfig _config;

        public BuyerRepository(IConfig config)
        {
            _config = config;
            Initialize();
        }

        public void Save(string itemId, Buyer buyer)
        {
            // Using ADO.NET here - where in real world application
            // one should consider using fully fledged framework such as
            // [Hibernate](https://hibernate.org/) or [Entity Framework](https://learn.microsoft.com/en-us/ef/).
            //string microsoft_sql = @"
            //    IF EXISTS (SELECT TOP 1 1 FROM dbo.[Buyer] WHERE [ItemID] = @ItemID)
            //    BEGIN
            //        UPDATE dbo.[Buyer]
            //        SET [BuyerName] = @BuyerName,
            //            [MaximumPrice] = @MaximumPrice,
            //            [NumberToBuy] = @NumberToBuy,
            //            [CurrentPrice] = @CurrentPrice,
            //            [NumberInStock] = @NumberInStock,
            //            [BoughtSoFar] = @BoughtSoFar,
            //            [State] = @State
            //        WHERE ItemID = @ItemID
            //    END
            //    ELSE
            //    BEGIN
            //        INSERT dbo.[Buyer] (ItemID, BuyerName, MaximumPrice, NumberToBuy,
            //            CurrentPrice, NumberInStock, BoughtSoFar, State)
            //        VALUES (@ItemID, @BuyerName, @MaximumPrice, @NumberToBuy,
            //            @CurrentPrice, @NumberInStock, @BoughtSoFar, @State)
            //    END";
            string mysql = @"
            IF EXISTS (SELECT 1 FROM `Buyer` WHERE `ItemID` = @ItemID LIMIT 1)
            THEN
                UPDATE `Buyer`
                SET `BuyerName` = @BuyerName,
                    `MaximumPrice` = @MaximumPrice,
                    `NumberToBuy` = @NumberToBuy,
                    `CurrentPrice` = @CurrentPrice,
                    `NumberInStock` = @NumberInStock,
                    `BoughtSoFar` = @BoughtSoFar,
                    `State` = @State
                WHERE ItemID = @ItemID;
            ELSE
                -- SQLINES FOR EVALUATION USE ONLY (14 DAYS)
                INSERT `Buyer` (ItemID, BuyerName, MaximumPrice, NumberToBuy,
                    CurrentPrice, NumberInStock, BoughtSoFar, State)
                VALUES (@ItemID, @BuyerName, @MaximumPrice, @NumberToBuy,
                    @CurrentPrice, @NumberInStock, @BoughtSoFar, @State);
            END IF;";

            using (var connection = new MySqlConnection(_config.Connection))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = mysql;
                command.Parameters.AddWithValue("@ItemID", itemId);
                foreach (var pair in buyer.ToDictionary())
                {
                    command.Parameters.AddWithValue($"@{pair.Key}", pair.Value);
                }
                command.ExecuteNonQuery();
            }
        }

        private void Initialize()
        {
            try
            {
                // test connection
                using (var connection = new MySqlConnection(_config.Connection))
                {
                    connection.Open();
                    CreateTable(connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SqlConnection");
            }
        }

        private static void CreateTable(MySqlConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS `autobuyer`.`buyer` (
                            `ItemID` TINYTEXT NOT NULL ,
                            `BuyerName` TINYTEXT NOT NULL ,
                            `MaximumPrice` INT NOT NULL ,
                            `NumberToBuy` INT NOT NULL ,
                            `CurrentPrice` INT NOT NULL ,
                            `NumberInStock` INT NOT NULL ,
                            `BoughtSoFar` INT NOT NULL ,
                            `State` INT NOT NULL ,
                            PRIMARY KEY (`ItemID`(20))) ENGINE = InnoDB;";
                command.ExecuteNonQuery();
            }
        }
    }
}
