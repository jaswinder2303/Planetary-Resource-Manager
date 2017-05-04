using System.Collections.Generic;
using PlanetaryResourceManager.Core.Models;

namespace PlanetaryResourceManager.Test
{
    static class FakeMarketOrderRepository
    {
        internal static void Initialize()
        {
            BuyOrders = new List<MarketOrder>
            {
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec)
            };

            SellOrders = new List<MarketOrder>
            {
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec),
                MakeOrder(2.2, 50, MarketDataResponse.HighSec)
            };
        }

        internal static List<MarketOrder> BuyOrders { get; private set; }

        internal static List<MarketOrder> SellOrders { get; private set; }

        private static MarketOrder MakeOrder(double price, int quantity, double security)
        {
            return new MarketOrder
            {
                Price = price,
                Quantity = quantity,
                Security = security + 0.1
            };
        }
    }
}
