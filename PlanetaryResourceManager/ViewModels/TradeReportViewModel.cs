using PlanetaryResourceManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.ViewModels
{
    class TradeReportViewModel : BaseViewModel
    {
        private TradeItem _currentItem;

        public TradeReportViewModel(List<TradeItem> tradeItems)
        {
            Commodities = new ObservableCollection<TradeItem>(tradeItems);
        }

        public  ObservableCollection<TradeItem> Commodities { get; set; }

        public TradeItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }
    }
}
