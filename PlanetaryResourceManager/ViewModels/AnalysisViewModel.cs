using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Data;
using PlanetaryResourceManager.Helpers;
using PlanetaryResourceManager.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace PlanetaryResourceManager.ViewModels
{
    public class AnalysisViewModel : INotifyPropertyChanged
    {
        private const string MinimumOrder = "500000";
        private const int MinimumQuanity = 400000;
        private Dictionary<string, int> _productionLevels;
        private EveRepository _repository;
        private List<AnalysisItem> _analysisItems;
        private int _currentProgress;
        private int _productionLevel;
        private bool _analysisInProgress;

        public AnalysisViewModel()
        {
            _repository = new EveRepository();
            _productionLevels = new Dictionary<string, int>{
                {"Raw Materials", 1},
                {"Processed Materials", 2},
                {"Refined Materials", 3}
            };

            ProductionLevels = new ObservableCollection<string>(_productionLevels.Keys);
            CurrentProductionLevel = "Refined Materials";

            AnalyzeCommand = new DelegateCommand(Analyze);
            LoadCommand = new DelegateCommand(LoadProductionItems);
            AnalysisItems = new ObservableCollection<AnalysisItem>();
            LoadProductionItems(null);
        }

        public ObservableCollection<AnalysisItem> AnalysisItems { get; set; }
        public ObservableCollection<string> ProductionLevels { get; set; }
        public string CurrentProductionLevel { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                RaisePropertyChanged("CurrentProgress");
                RaisePropertyChanged("ProgressText");
            }
        }

        public string ProgressText
        {
            get { return string.Format("{0}%", _currentProgress); }
        }

        public bool AnalysisInProgress
        {
            get { return _analysisInProgress; }
            set
            {
                _analysisInProgress = value;
                RaisePropertyChanged("AnalysisInProgress");
            }
        }

        private void LoadProductionItems(object arg)
        {
            AnalysisItems.Clear();
            _productionLevel = _productionLevels[CurrentProductionLevel];
            _analysisItems = _repository.GetProductionItems(_productionLevel);
            _analysisItems.ForEach(item => AnalysisItems.Add(item));
            //AnalysisItems = new ObservableCollection<AnalysisItem>(_analysisItems);
        }

        private async void Analyze(object obj)
        {
            CurrentProgress = 0;
            AnalysisInProgress = true;

            var progress = new Progress<int>(percent =>
            {
                CurrentProgress = percent;
            });

            await Task.Run(() => Analyze(progress));
        }

        private void Analyze(IProgress<int> progress)
        {
            MarketDataHelper helper = new MarketDataHelper("http://api.eve-central.com/api/quicklook");
            int index = 0;

            foreach (var item in AnalysisItems)
            {
                MarketDataRequest request = new MarketDataRequest
                {
                    TypeId = item.Product.ItemId.ToString()
                };

                var response = helper.GetData(request);
                var order = response.HighestBuyOrder;
                item.Product.Price = order != null ? order.Price : 0.0;
                item.Product.ExportCost = ProductionHelper.GetExportCost(_productionLevel);
                item.Product.InputBatchSize = ProductionHelper.GetInputBatchSize(_productionLevel);
                item.Product.OutputBatchSize = ProductionHelper.GetOutputBatchSize(_productionLevel);

                foreach (var input in item.Materials)
                {
                    request = new MarketDataRequest
                    {
                        TypeId = input.ItemId.ToString(),
                        MinimumQuantity = AnalysisViewModel.MinimumOrder
                    };

                    response = helper.GetData(request);
                    order = response.LowestSellOrder(AnalysisViewModel.MinimumQuanity);
                    input.Price = order != null ? order.Price : 0.0;
                    input.ImportCost = ProductionHelper.GetImportCost(_productionLevel);
                }

                var productionResult = ProductionHelper.Calculate(item.Product, item.Materials);
                item.ProductionCost = productionResult.PurchaseCost;
                item.SaleValue = productionResult.SaleCost;
                item.ProfitMargin = productionResult.ProfitMargin;
                item.UpdateProperties();

                var currentProgress = ((double)++index / AnalysisItems.Count) * 100;
                progress.Report((int)currentProgress);
            }

            AnalysisInProgress = false;
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
