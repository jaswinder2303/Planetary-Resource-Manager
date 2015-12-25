using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Core.Helpers;
using PlanetaryResourceManager.Core.Models;
using PlanetaryResourceManager.Views;

namespace PlanetaryResourceManager.ViewModels
{
    public class ManufactureViewModel : INotifyPropertyChanged
    {
        public Product TargetProduct { get; set; }
        public RawMaterial InputA { get; set; }
        public RawMaterial InputB { get; set; }
        public RawMaterial InputC { get; set; }
        public bool RequiresThirdInput { get; set; }
        public int BatchSize { get; set; }
        public string InputQuantity { get; set; }
        public int OutputQuantity { get; set; }
        public double PurchaseCost { get; set; }
        public double SaleCost { get; set; }
        public double Expenses { get; set; }
        public double ProfitMargin { get; set; }
        public ICommand CalculateCommand { get; set; }
        public ICommand ListOrdersCommand { get; set; }
        public int ProductionLevel { get; set; }

        public ManufactureViewModel() : this(null)
        {
            
        }

        public ManufactureViewModel(AnalysisItem item)
        {
            if (item == null)
            {
                item = new AnalysisItem
                {
                    ProductionLevel = 3,
                    Product = new Product
                    {
                        Name = "Microfiber Sheilding",
                        Price = 11800.00,
                        OutputBatchSize = 5,
                        ExportCost = 1224
                    },
                    Materials = new List<RawMaterial>{
                        new RawMaterial
                        {
                            Name = "Silicon",
                            Price = 600,
                            InputBatchSize = 40,
                            ImportCost = 34
                        },
                        new RawMaterial
                        {
                            Name = "Industrial Fiber",
                            Price = 500,
                            InputBatchSize = 40,
                            ImportCost = 34
                        }
                    }
                };
            }

            TargetProduct = item.Product;
            InputA = item.Materials[0];
            InputB = item.Materials[1];
            ProductionLevel = item.ProductionLevel;

            if (item.Materials.Count == 3)
            {
                InputC = item.Materials[2];
                RequiresThirdInput = true;
            }

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
            var materials = new List<RawMaterial> { InputA, InputB };
            if (InputC != null)
            {
                materials.Add(InputC);
            }

            TargetProduct.OutputBatchSize = ProductionHelper.GetOutputBatchSize(ProductionLevel);
            foreach (var material in materials)
            {
                material.ImportCost = ProductionHelper.GetImportCost(material.InputLevel);
                material.InputBatchSize = ProductionHelper.GetInputBatchSize(material.InputLevel);
            }

            var result = ProductionHelper.Calculate(TargetProduct, materials, BatchSize);
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
            RaisePropertyChanged("RequiresThirdInput");
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
