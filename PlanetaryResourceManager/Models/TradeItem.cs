using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class TradeItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public MarketDataResponse Data { get; set; }

        public double ProfitMargin
        {
            get
            {
                if (Data != null && Data.HighestBuyOrder != null && Data.LowestSellOrder(null) != null)
                {
                    return Data.HighestBuyOrder.Price - Data.LowestSellOrder(null).Price;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        internal void Update(double securityLevel)
        {
            if (Data != null)
            {
                Data.SecurityLevel = securityLevel;
                Data.SellOrders = Data.SellOrders.OrderBy(arg => arg.Price).ToList();
                Data.BuyOrders = Data.BuyOrders.OrderByDescending(arg => arg.Price).ToList();
            }
        }
    }
}
