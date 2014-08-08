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

        public MarketOrder LowestSellOrder(int? minimumQuantity)
        {
            MarketOrder item = null;

            if (SellOrders != null)
            {
                int filter = 0;

                if (minimumQuantity.HasValue)
                {
                    filter = minimumQuantity.Value > UseableQuantity ? minimumQuantity.Value : UseableQuantity;
                }

                item = SellOrders.Where(arg => arg.Quantity > filter).FirstOrDefault();
            }

            return item;
        }

        public MarketOrder HighestBuyOrder
        {
            get
            {
                MarketOrder item = null;

                if (BuyOrders != null)
                {
                    item = BuyOrders.FirstOrDefault();
                }

                return item;
            }
        }

        public List<MarketOrder> BestBuyers
        {
            get
            {
                return BuyOrders.Take(10).ToList();
            }
        }

        public List<MarketOrder> BestSellers
        {
            get
            {
                return SellOrders.Take(10).ToList();
            }
        }
    }
}
