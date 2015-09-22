using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Models
{
    public class ProductionResult
    {
        public string InputQuantity { get; set; }
        public int OutputQuantity { get; set; }
        public double PurchaseCost { get; set; }
        public double SaleCost { get; set; }
        public double Expenses { get; set; }
        public double ProfitMargin { get; set; }
        public Dictionary<int, int> InputQuantities { get; set; }
    }
}
