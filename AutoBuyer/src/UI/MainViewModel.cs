﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoBuyer.Logic.Connection;
using AutoBuyer.Logic.Domain;

namespace AutoBuyer.UI
{
    public class MainViewModel
    {
        private readonly string _buyerName;
        private readonly IWarehouseConnection _connection;

        public ObservableCollection<BuyerViewModel> Buyers { get; }
        public Command StartBuyingCommand { get; private set; }

        public string NewItemId { get; set; }
        public int NewItemMaximumPrice { get; set; }
        public int NumberToBuy { get; set; }

        public MainViewModel(string buyerName, IWarehouseConnection connection)
        {
            _buyerName = buyerName;
            _connection = connection;
            StartBuyingCommand = new Command(Join);
            Buyers = new ObservableCollection<BuyerViewModel>();
        }

        private void Join()
        {
            IStockItemConnection itemConnection = _connection.ConnectToItem(NewItemId, _buyerName);
            var vm = new BuyerViewModel(NewItemId, NewItemMaximumPrice, NumberToBuy, _buyerName, itemConnection);
            Buyers.Add(vm);
        }
    }
}
