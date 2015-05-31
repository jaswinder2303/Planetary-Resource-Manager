using LoggingUtility;
using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Data;
using PlanetaryResourceManager.Events;
using PlanetaryResourceManager.Helpers;
using PlanetaryResourceManager.Models;
using PlanetaryResourceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlanetaryResourceManager.ViewModels
{
    class TradeAnalysisViewModel : BaseViewModel
    {
        private bool _analysisInProgress;
        private int _currentProgress;
        private EveRepository _repository;
        private TradeCategory _currentCategory;
        private List<TradeCategory> _categories;
        private Dictionary<string, double> _securityLevels;
        private string _currentSecurityLevel;

        public TradeAnalysisViewModel(EveRepository repository)
        {
            _repository = repository;
            _categories = _repository.GetTradeCategories();
            _currentSecurityLevel = "All";
            _securityLevels = new Dictionary<string, double>{
                {"All", -1},
                {"Low Sec", 0.1},
                {"High Sec", 0.5}
            };

            SecurityLevels = new ObservableCollection<string>(_securityLevels.Keys);
            TradeCategories = new ObservableCollection<TradeCategory>(_categories);
            TradeGroups = new ObservableCollection<TradeGroup>();
            AnalyzeCommand = new DelegateCommand(Analyze);
            ShowDetails = new DelegateCommand(ShowMarketDetails);
            StartAllCommand = new DelegateCommand(StartAll);
        }

        private LogUtility Logger { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand StartAllCommand { get; set; }
        public ICommand ShowDetails { get; set; }
        public ObservableCollection<TradeCategory> TradeCategories { get; set; }
        public ObservableCollection<TradeGroup> TradeGroups { get; set; }
        public ObservableCollection<string> SecurityLevels { get; set; }

        public string CurrentSecurityLevel
        {
            get { return _currentSecurityLevel; }
            set
            {
                _currentSecurityLevel = value;

                if (CurrentCategory != null)
                {
                    _categories.ForEach(cat => cat.Groups.ForEach(group => group.Update(_securityLevels[_currentSecurityLevel])));
                    LoadTradeGroups(CurrentCategory);
                }
            }
        }

        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                RaisePropertyChanged("CurrentProgress");
            }
        }

        public bool IsAnalysisInProgress
        {
            get { return _analysisInProgress; }
            set
            {
                _analysisInProgress = value;
                RaisePropertyChanged("IsAnalysisInProgress");
            }
        }

        public TradeCategory CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                _currentCategory = value;
                LoadTradeGroups(_currentCategory);
            }
        }

        private void LoadTradeGroups(TradeCategory category)
        {
            if (category != null)
            {
                TradeGroups.Clear();
                category.Groups.OrderByDescending(member => member.ProfitMargin).ToList().ForEach(arg => TradeGroups.Add(arg));
            }
        }

        private async void StartAll(object obj)
        {
            CurrentProgress = 0;
            IsAnalysisInProgress = true;
            Logger = LogUtility.CreateLogger("PlanetaryResourceManager", null, null, LogLevel.All, LogMode.Explicit);

            var progress = new Progress<int>(percent =>
            {
                CurrentProgress = percent;

                ProgressManager.ReportProgress(CurrentProgress);

                if (CurrentProgress == 100)
                {
                    LoadTradeGroups(CurrentCategory);
                }
            });

            Logger.AddTimedTrace("Staring All Trade analysis");

            await Task.Factory.StartNew(() => AnalyzeAll(progress)).ContinueWith((task) =>
            {
                Logger.AddTimedTrace("Completed All Trade analysis");
                Logger.WriteMessagesToFile();
            });
        }

        private async void Analyze(object obj)
        {
            CurrentProgress = 0;
            IsAnalysisInProgress = true;
            Logger = LogUtility.CreateLogger("PlanetaryResourceManager", null, null, LogLevel.All, LogMode.Explicit);

            var progress = new Progress<int>(percent =>
            {
                CurrentProgress = percent;

                ProgressManager.ReportProgress(CurrentProgress);

                if (CurrentProgress == 100)
                {
                    LoadTradeGroups(CurrentCategory);
                }
            });

            Logger.AddTimedTrace("Staring Trade analysis");

            await Task.Factory.StartNew(() => Analyze(progress)).ContinueWith((task) =>
            {
                Logger.AddTimedTrace("Completed Trade analysis");
                Logger.WriteMessagesToFile();
            });
        }

        private void ShowMarketDetails(object arg)
        {
            var item = arg as TradeGroup;

            if (item != null)
            {
                var viewModel = new TradeReportViewModel(item.Items);
                var dialog = new TradingReportWindow();
                dialog.DataContext = viewModel;
                dialog.ShowDialog();
            }
        }

        private void Analyze(IProgress<int> progress)
        {
            int index = 0;
            var categoryTasks = new List<Task>();

            foreach (var group in _currentCategory.Groups)
            {
                categoryTasks.Add(Task.Factory.StartNew(() =>
                {
                    var groupTasks = new List<Task>();

                    foreach (var item in group.Items)
                    {
                        groupTasks.Add(Task.Factory.StartNew(() => {
                            using (MarketDataHelper helper = new MarketDataHelper(MarketDataHelper.QuickLook))
                            {
                                MarketDataRequest request = new MarketDataRequest
                                {
                                    TypeId = item.Id.ToString(),
                                    Duration = MarketDataHelper.Freshness
                                };

                                item.Data = helper.GetData(request);
                            }
                        }, TaskCreationOptions.AttachedToParent));
                    }

                    Task.Factory.ContinueWhenAll(groupTasks.ToArray(), groupedTasks => {
                        group.Update(_securityLevels[CurrentSecurityLevel]);
                        var currentProgress = ((double)++index / _currentCategory.Groups.Count) * 100;
                        progress.Report((int)currentProgress);
                    });

                }, TaskCreationOptions.AttachedToParent));
            }

            Task.Factory.ContinueWhenAll(categoryTasks.ToArray(), categorizedTasks => {
                IsAnalysisInProgress = false;
                _currentCategory.IsProcessed = true;
            }); 
        }

        private void AnalyzeAll(IProgress<int> progress)
        {
            int index = 0;
            var tradeTasks = new List<Task>();

            foreach (var category in _categories)
            {
                tradeTasks.Add(Task.Factory.StartNew(() =>
                {
                    var categoryTasks = new List<Task>();

                    foreach (var group in category.Groups)
                    {
                        categoryTasks.Add(Task.Factory.StartNew(() =>
                        {
                            var groupTasks = new List<Task>();

                            foreach (var item in group.Items)
                            {
                                groupTasks.Add(Task.Factory.StartNew(() =>
                                {
                                    using (MarketDataHelper helper = new MarketDataHelper(MarketDataHelper.QuickLook))
                                    {
                                        MarketDataRequest request = new MarketDataRequest
                                        {
                                            TypeId = item.Id.ToString(),
                                            Duration = MarketDataHelper.Freshness
                                        };

                                        item.Data = helper.GetData(request);
                                    }
                                }, TaskCreationOptions.AttachedToParent));
                            }

                            if (groupTasks.Any())
                            {
                                Task.Factory.ContinueWhenAll(groupTasks.ToArray(), groupedTasks =>
                                {
                                    group.Update(_securityLevels[CurrentSecurityLevel]);
                                });
                            }

                        }, TaskCreationOptions.AttachedToParent));
                    }

                    if (categoryTasks.Any())
                    {
                        Task.Factory.ContinueWhenAll(categoryTasks.ToArray(), categorizedTasks =>
                        {
                            category.IsProcessed = true;
                            var currentProgress = ((double)++index / _categories.Count) * 100;
                            progress.Report((int)currentProgress);
                        });
                    }
                    else
                    {
                        category.IsProcessed = true;
                        var currentProgress = ((double)++index / _categories.Count) * 100;
                        progress.Report((int)currentProgress);
                    }

                }, TaskCreationOptions.AttachedToParent));
            }

            Task.Factory.ContinueWhenAll(tradeTasks.ToArray(), tradingTasks => {
                IsAnalysisInProgress = false;
            });  
        }
    }
}
