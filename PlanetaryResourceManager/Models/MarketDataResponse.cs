using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class MarketDataResponse
    {
        const int UseableQuantity = 250000;

        public string Commodity { get; set; }
        public List<MarketOrder> BuyOrders { get; set; }
        public List<MarketOrder> SellOrders { get; set; }

        public MarketOrder LowestSellOrder(int minimumQuantity)
        {
            int filter = minimumQuantity > UseableQuantity ? minimumQuantity : UseableQuantity;

            var item = SellOrders.Where(arg => arg.Quantity > filter).FirstOrDefault();

            return item;
        }

        public MarketOrder HighestBuyOrder
        {
            get
            {
                return BuyOrders.FirstOrDefault();
            }
        }
    }
}
