using System;
using System.ComponentModel;
using PlanetaryResourceManager.Core.Data;

namespace PlanetaryResourceManager.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly EveRepository _repository;
        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseViewModel() : this(new EveRepository())
        {

        }

        protected BaseViewModel(EveRepository repository)
        {
            _repository = repository;
        }

        protected EveRepository Repository => _repository;

        protected void RaisePropertyChanged(string property)
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
