using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Data;
using PlanetaryResourceManager.Helpers;
using PlanetaryResourceManager.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;

namespace PlanetaryResourceManager.ViewModels
{
    public class AnalysisViewModel : INotifyPropertyChanged
    {
        public ICommand AnalyzeCommand { get; set; }
        private const string MinimumOrder = "500000";
        private const int MinimumQuanity = 400000;

        public AnalysisViewModel()
        {
            AnalyzeCommand = new DelegateCommand(Analyze);
        }

        private void Analyze(object obj)
        {
            EveRepository repository = new EveRepository();
            var items = repository.GetProductionItems(3);

            MarketDataHelper helper = new MarketDataHelper("http://api.eve-central.com/api/quicklook");

            foreach (var item in items)
            {
                MarketDataRequest request = new MarketDataRequest
                {
                    TypeId = item.Product.ItemId.ToString()
                };

                var response = helper.GetData(request);
                var order = response.HighestBuyOrder;
                item.Product.Price = order != null ? order.Price : 0.0;

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
                }
            }

            var bestSeller = items.OrderByDescending(arg => arg.Product.Price).First();
            //MarketDataRequest request = new MarketDataRequest{
            //    Duration = "24",
            //    MinimumQuantity = "500000",
            //    SystemId = "30000142",
            //    TypeId = "9828"
            //};

            //var response = helper.GetData(request);
            //var buyOrder = response.HighestBuyOrder;
            //var sellOrder = response.LowestSellOrder(400000);
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
