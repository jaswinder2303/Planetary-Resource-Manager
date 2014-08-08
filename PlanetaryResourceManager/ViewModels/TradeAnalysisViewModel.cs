﻿using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Data;
using PlanetaryResourceManager.Helpers;
using PlanetaryResourceManager.Models;
using PlanetaryResourceManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        public TradeAnalysisViewModel(EveRepository repository)
        {
            _repository = repository;
            _categories = _repository.GetTradeCategories();
            TradeCategories = new ObservableCollection<TradeCategory>(_categories);
            TradeGroups = new ObservableCollection<TradeGroup>();

            AnalyzeCommand = new DelegateCommand(Analyze);
            ShowDetails = new DelegateCommand(ShowMarketDetails);
        }

        public ICommand AnalyzeCommand { get; set; }
        public ICommand ShowDetails { get; set; }
        public ObservableCollection<TradeCategory> TradeCategories { get; set; }
        public ObservableCollection<TradeGroup> TradeGroups { get; set; }

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

        private async void Analyze(object obj)
        {
            CurrentProgress = 0;
            IsAnalysisInProgress = true;

            var progress = new Progress<int>(percent =>
            {
                CurrentProgress = percent;

                if (CurrentProgress == 100)
                {
                    LoadTradeGroups(CurrentCategory);
                }
            });

            await Task.Run(() => Analyze(progress));
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
            MarketDataHelper helper = new MarketDataHelper(MarketDataHelper.QuickLook);
            int index = 0;

            foreach (var group in _currentCategory.Groups)
            {
                foreach (var item in group.Items)
                {
                    MarketDataRequest request = new MarketDataRequest
                    {
                        TypeId = item.Id.ToString(),
                        Duration = MarketDataHelper.Freshness
                    };

                    item.Data = helper.GetData(request);
                }

                group.Update();
                var currentProgress = ((double)++index / _currentCategory.Groups.Count) * 100;
                progress.Report((int)currentProgress);
            }

            IsAnalysisInProgress = false;
        }
    }
}
