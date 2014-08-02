using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Models;
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


        public ManufactureViewModel()
        {
            TargetProduct = new Product
            {
                Name = "Microfiber Sheilding",
                Price = 11800.00,
                InputBatchSize = 40,
                OutputBatchSize = 5,
                ExportCost = 1224
            };

            InputA = new RawMaterial
            {
                Name = "Silicon",
                Price = 600,
                ImportCost = 34
            };

            InputB = new RawMaterial
            {
                Name = "Industrial Fiber",
                Price = 500,
                ImportCost = 34
            };

            BatchSize = 8000;
            Expenses = 60000000;

            CalculateCommand = new DelegateCommand(Calculate);

            Calculate(null);
        }

        private void Calculate(object arg)
        {
            InputQuantity = TargetProduct.InputBatchSize * BatchSize;
            OutputQuantity = TargetProduct.OutputBatchSize * BatchSize;
            SaleCost = TargetProduct.Price * OutputQuantity;
            Expenses = (InputQuantity * InputA.ImportCost) + (InputQuantity * InputB.ImportCost) + (OutputQuantity * TargetProduct.ExportCost);
            PurchaseCost = (InputA.Price * InputQuantity) + (InputB.Price * InputQuantity);
            ProfitMargin = SaleCost - (PurchaseCost + Expenses);

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
