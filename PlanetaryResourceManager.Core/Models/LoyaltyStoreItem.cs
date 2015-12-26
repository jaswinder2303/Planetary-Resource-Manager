using System.ComponentModel;

namespace PlanetaryResourceManager.Core.Models
{
    public class LoyaltyStoreItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int ItemId { get; set; }
        public double Points { get; set; }
        public double StorePrice { get; set; }
        public double MarketPrice { get; set; }
        public double ProfitMargin { get; set; }
        public double ProfitEfficiency { get; set; }
        public Product Product { get; set; }

        public void UpdateProperties()
        {
            RaisePropertyChanged("MarketPrice");
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
