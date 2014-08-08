using PlanetaryResourceManager.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.ViewModels
{
    class ManagementViewModel : INotifyPropertyChanged
    {
        public ManagementViewModel()
        {
            EveRepository repository = new EveRepository();
            ProductionViewModel = new AnalysisViewModel(repository);
            TradeViewModel = new TradeAnalysisViewModel(repository);
        }

        public AnalysisViewModel ProductionViewModel { get; set; }
        public TradeAnalysisViewModel TradeViewModel { get; set; }

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
