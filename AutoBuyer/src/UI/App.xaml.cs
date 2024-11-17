using AutoBuyer.Logic;
using AutoBuyer.Logic.Connection;

namespace AutoBuyer.UI
{
    public partial class App
    {
        private const string BuyerName = "Buyer";

        public App()
        {
            var connection = new WarehouseConnection();

            var mainViewModel = new MainViewModel(BuyerName, connection);

            var window = new MainWindow
            {
                DataContext = mainViewModel
            };
            window.ShowDialog();
        }
    }
}
