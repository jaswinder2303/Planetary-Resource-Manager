using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class ProductionResult
    {
        public int InputQuantity { get; set; }
        public int OutputQuantity { get; set; }
        public double PurchaseCost { get; set; }
        public double SaleCost { get; set; }
        public double Expenses { get; set; }
        public double ProfitMargin { get; set; }
    }
}
