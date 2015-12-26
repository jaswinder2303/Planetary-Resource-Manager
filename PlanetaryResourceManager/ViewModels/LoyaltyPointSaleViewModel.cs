using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Core.Data;
using PlanetaryResourceManager.Core.Events;
using PlanetaryResourceManager.Core.Helpers;
using PlanetaryResourceManager.Core.Models;
using PlanetaryResourceManager.Views;
using Commodity = PlanetaryResourceManager.Core.Models.Commodity;

namespace PlanetaryResourceManager.ViewModels
{
    class LoyaltyPointSaleViewModel : BaseViewModel
    {
        private readonly List<LoyaltyStoreItem> _storeItems;
        private readonly Dictionary<string, int> _pageSizes;
        private int _currentProgress;
        private int _currentPage;
        private string _currentPageSize;
        private bool _analysisInProgress;
        private bool _canGoBackward;
        private bool _canGoForward;

        public LoyaltyPointSaleViewModel(EveRepository repository) : base(repository)
        {
            _pageSizes = new Dictionary<string, int>
            {
                {"20", 20},
                {"50", 50},
                {"100", 100},
                {"All", -1}
            };
            _currentPageSize = "All";
            PageSizes = new ObservableCollection<string>(_pageSizes.Keys);
            AnalyzeCommand = new DelegateCommand(Analyze);
            NextCommand = new DelegateCommand(GoForward);
            PreviousCommand = new DelegateCommand(GoBack);
            ShowDetails = new DelegateCommand(ListOrders);
            _storeItems = Repository.GetLoyaltyStoreItems();
            StoreItems = new ObservableCollection<LoyaltyStoreItem>();
            RebuildList();
        }

        public ObservableCollection<string> PageSizes { get; set; }
        public ObservableCollection<LoyaltyStoreItem> StoreItems { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand ShowDetails { get; set; }

        public string CurrentPageSize
        {
            get { return _currentPageSize; }
            set
            {
                _currentPageSize = value;
                _currentPage = 0;
                RebuildList();
            }
        }

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

        public bool CanGoBackward
        {
            get { return _canGoBackward; }
            set
            {
                _canGoBackward = value;
                RaisePropertyChanged("CanGoBackward");
            }
        }

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set
            {
                _canGoForward = value;
                RaisePropertyChanged("CanGoForward");
            }
        }

        private void GoBack(object arg)
        {
            --_currentPage;
            RebuildList();
        }

        private void GoForward(object arg)
        {
            ++_currentPage;
            RebuildList();
        }

        private void ListOrders(object arg)
        {
            var storeItem = arg as LoyaltyStoreItem;

            if (storeItem != null)
            {
                var ordersList = new List<TradeItem>
                {
                    new TradeItem(){
                        Name = storeItem.Name,
                        Data = storeItem.Product.Data
                    }
                };

                var viewModel = new TradeReportViewModel(ordersList);
                viewModel.CurrentItem = viewModel.Commodities.First();
                var dialog = new TradingReportWindow();
                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
        }

        private void RebuildList()
        {
            StoreItems.Clear();
            var pageSize = _pageSizes[CurrentPageSize];
            var previousPage = _currentPage * pageSize;

            if (pageSize < 0)
            {
                pageSize = _storeItems.Count;
                CanGoForward = false;
            }
            else
            {
                CanGoForward = ((_currentPage + 1) * pageSize) < _storeItems.Count;
            }

            CanGoBackward = _currentPage > 0;
            
            _storeItems.OrderByDescending(member => member.ProfitEfficiency).Skip(previousPage).Take(pageSize).ToList().ForEach(item => StoreItems.Add(item));
        }

        private async void Analyze(object obj)
        {
            CurrentProgress = 0;
            AnalysisInProgress = true;

            var progress = new Progress<int>(percent =>
            {
                CurrentProgress = percent;

                ProgressManager.ReportProgress(CurrentProgress);

                if (CurrentProgress == 100)
                {
                    RebuildList();
                }
            });

            await Task.Factory.StartNew(() => Analyze(progress)).ContinueWith((task) =>
            {

            });
        }

        private void Analyze(IProgress<int> progress)
        {
            int index = 0;
            var itemTasks = new List<Task>();

            foreach (var item in _storeItems)
            {
                itemTasks.Add(Task.Factory.StartNew(() =>
                {
                    using (MarketDataHelper helper = new MarketDataHelper(MarketDataHelper.QuickLook))
                    {
                        MarketDataRequest request = new MarketDataRequest
                        {
                            TypeId = item.ItemId.ToString(),
                            SystemId = MarketDataHelper.Jita,
                            Duration = MarketDataHelper.Freshness
                        };

                        var productData = helper.GetData(request);
                        MarketDataResponse.ResequenceOrders(productData);
                        var order = productData.HighestBuyOrder;
                        item.Product.Data = productData;
                        item.Product.Price = order != null ? order.Price : 0.0;
                        item.MarketPrice = order != null ? order.Price : 0.0;
                        item.ProfitMargin = item.MarketPrice - item.StorePrice;
                        item.ProfitEfficiency = item.ProfitMargin/item.Points;
                        item.UpdateProperties();

                        var currentProgress = ((double)++index / _storeItems.Count) * 100;
                        progress.Report((int)currentProgress);
                    }
                }, TaskCreationOptions.AttachedToParent));
            }

            Task.Factory.ContinueWhenAll(itemTasks.ToArray(), groupedTasks => {
                AnalysisInProgress = false;
            });
        }
    }
}
