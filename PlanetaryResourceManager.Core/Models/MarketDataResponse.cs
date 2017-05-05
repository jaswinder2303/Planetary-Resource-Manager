using System.Collections.Generic;
using System.Linq;

namespace PlanetaryResourceManager.Core.Models
{
    public class MarketDataResponse
    {
        private const int UseableQuantity = 2500;
        public const double NullSec = -1;
        public const double LowSec = 0.0;
        public const double HighSec = 0.4;

        public MarketDataResponse()
        {
            SecurityLevel = NullSec;
        }

        public string Commodity { get; set; }
        public List<MarketOrder> BuyOrders { get; set; }
        public List<MarketOrder> SellOrders { get; set; }
        public double SecurityLevel { get; set; }

        public MarketOrder LowestSellOrder(int? minimumQuantity)
        {
            MarketOrder item = null;

            if (SellOrders != null)
            {
                var filter = minimumQuantity ?? UseableQuantity;

                item = SellOrders.FirstOrDefault(arg => arg.Quantity > filter && arg.Security > SecurityLevel);
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
                    item = BuyOrders.FirstOrDefault(arg => arg.Security > SecurityLevel);
                }

                return item;
            }
        }

        public List<MarketOrder> BestBuyers
        {
            get
            {
                return BuyOrders.Where(arg => arg.Security > SecurityLevel).Take(75).ToList();
            }
        }

        public List<MarketOrder> BestSellers
        {
            get
            {
                return SellOrders.Where(arg => arg.Quantity > UseableQuantity && arg.Security > SecurityLevel).Take(75).ToList();
            }
        }

        public static void ResequenceOrders(MarketDataResponse data)
        {
            data.SellOrders = data.SellOrders.OrderBy(arg => arg.Price).ToList();
            data.BuyOrders = data.BuyOrders.OrderByDescending(arg => arg.Price).ToList();
        }
    }
}
