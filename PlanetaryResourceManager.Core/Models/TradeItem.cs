using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Models
{
    public class TradeItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public MarketDataResponse Data { get; set; }

        public double ProfitMargin
        {
            get
            {
                if (Data != null && Data.HighestBuyOrder != null && Data.LowestSellOrder(2500) != null)
                {
                    return Data.HighestBuyOrder.Price - Data.LowestSellOrder(2500).Price;
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
                MarketDataResponse.ResequenceOrders(Data);
            }
        }
    }
}
