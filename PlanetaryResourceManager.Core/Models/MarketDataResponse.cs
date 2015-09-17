using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Models
{
    public class MarketDataResponse
    {
        private const int UseableQuantity = 250000;
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
                int filter = 0;

                if (minimumQuantity.HasValue)
                {
                    filter = minimumQuantity.Value > UseableQuantity ? minimumQuantity.Value : UseableQuantity;
                }

                item = SellOrders.Where(arg => arg.Quantity > filter && arg.Security > SecurityLevel).FirstOrDefault();
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
                    item = BuyOrders.Where(arg => arg.Security > SecurityLevel).FirstOrDefault();
                }

                return item;
            }
        }

        public List<MarketOrder> BestBuyers
        {
            get
            {
                return BuyOrders.Where(arg => arg.Security > SecurityLevel).Take(50).ToList();
            }
        }

        public List<MarketOrder> BestSellers
        {
            get
            {
                return SellOrders.Where(arg => arg.Security > SecurityLevel).Take(50).ToList();
            }
        }

        internal static void ResequenceOrders(MarketDataResponse data)
        {
            data.SellOrders = data.SellOrders.OrderBy(arg => arg.Price).ToList();
            data.BuyOrders = data.BuyOrders.OrderByDescending(arg => arg.Price).ToList();
        }
    }
}
