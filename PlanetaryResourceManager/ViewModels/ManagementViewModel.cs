using PlanetaryResourceManager.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.ViewModels
{
    class ManagementViewModel : INotifyPropertyChanged, IDisposable
    {
        private EveRepository _repository;

        public ManagementViewModel()
        {
            _repository = new EveRepository();
            ProductionViewModel = new AnalysisViewModel(_repository);
            TradeViewModel = new TradeAnalysisViewModel(_repository);
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

        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
    }
}
