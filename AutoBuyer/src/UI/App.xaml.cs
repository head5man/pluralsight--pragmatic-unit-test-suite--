using AutoBuyer.Logic.Connection;
using AutoBuyer.Logic.Database;

namespace AutoBuyer.UI
{
    public partial class App
    {
        private const string BuyerName = "Buyer";

        public App()
        {
            var config = ConfigBuilder.Build();
            var connection = new WarehouseConnection();
            var database = new BuyerRepository(config);
            var mainViewModel = new MainViewModel(BuyerName, connection, database);

            var window = new MainWindow
            {
                DataContext = mainViewModel
            };
            window.ShowDialog();
        }
    }
}
