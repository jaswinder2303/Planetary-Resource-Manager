using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Helpers;
using PlanetaryResourceManager.Models;
using PlanetaryResourceManager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlanetaryResourceManager.ViewModels
{
    public class ManufactureViewModel : INotifyPropertyChanged
    {
        public Product TargetProduct { get; set; }
        public RawMaterial InputA { get; set; }
        public RawMaterial InputB { get; set; }
        public int BatchSize { get; set; }
        public int InputQuantity { get; set; }
        public int OutputQuantity { get; set; }
        public double PurchaseCost { get; set; }
        public double SaleCost { get; set; }
        public double Expenses { get; set; }
        public double ProfitMargin { get; set; }
        public ICommand CalculateCommand { get; set; }
        public ICommand ListOrdersCommand { get; set; }

        public ManufactureViewModel() : this(null)
        {
            
        }

        public ManufactureViewModel(AnalysisItem item)
        {
            if (item == null)
            {
                item = new AnalysisItem
                {
                    Product = new Product
                    {
                        Name = "Microfiber Sheilding",
                        Price = 11800.00,
                        InputBatchSize = 40,
                        OutputBatchSize = 5,
                        ExportCost = 1224
                    },
                    Materials = new List<RawMaterial>{
                        new RawMaterial
                        {
                            Name = "Silicon",
                            Price = 600,
                            ImportCost = 34
                        },
                        new RawMaterial
                        {
                            Name = "Industrial Fiber",
                            Price = 500,
                            ImportCost = 34
                        }
                    }
                };
            }

            TargetProduct = item.Product;
            InputA = item.Materials[0];
            InputB = item.Materials[1];
            BatchSize = ProductionHelper.BatchSize;
            ListOrdersCommand = new DelegateCommand(ListOrders);
            CalculateCommand = new DelegateCommand(Calculate);
            Calculate(null);
        }

        private void ListOrders(object arg)
        {
            Commodity commodity = arg as Commodity;

            if (commodity != null)
            {
                var ordersList = new List<TradeItem>
                {
                    new TradeItem(){
                        Name = commodity.Name,
                        Data = commodity.Data
                    }
                };

                var viewModel = new TradeReportViewModel(ordersList);
                viewModel.CurrentItem = viewModel.Commodities.First();
                var dialog = new TradingReportWindow();
                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
        }

        private void Calculate(object arg)
        {
            var result = ProductionHelper.Calculate(TargetProduct, new List<RawMaterial> { InputA, InputB }, BatchSize);
            InputQuantity = result.InputQuantity;
            OutputQuantity = result.OutputQuantity;
            SaleCost = result.SaleCost;
            Expenses = result.Expenses;
            PurchaseCost = result.PurchaseCost;
            ProfitMargin = result.ProfitMargin;

            RaisePropertyChanged("InputQuantity");
            RaisePropertyChanged("OutputQuantity");
            RaisePropertyChanged("SaleCost");
            RaisePropertyChanged("PurchaseCost");
            RaisePropertyChanged("Expenses");
            RaisePropertyChanged("ProfitMargin");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
