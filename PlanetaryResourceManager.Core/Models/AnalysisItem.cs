using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Models
{
    public class AnalysisItem : INotifyPropertyChanged
    {
        public Product Product { get; set; }
        public List<RawMaterial> Materials { get; set; }
        public int ProductionLevel { get; set; }
        public double SaleValue { get; set; }
        public double ProductionCost { get; set; }
        public double ProfitMargin { get; set; }

        public void UpdateProperties()
        {
            RaisePropertyChanged("SaleValue");
            RaisePropertyChanged("ProductionCost");
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
