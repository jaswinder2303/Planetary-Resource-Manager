using NUnit.Framework;
using PlanetaryResourceManager.Core.Models;

namespace PlanetaryResourceManager.Test
{
    [TestFixture]
    public class MarketDataResponseTest
    {
        private MarketDataResponse _dataResponse;

        [SetUp]
        public void Initialize()
        {
            FakeMarketOrderRepository.Initialize();

            _dataResponse = new MarketDataResponse
            {
                BuyOrders = FakeMarketOrderRepository.BuyOrders,
                SellOrders = FakeMarketOrderRepository.SellOrders
            };

            MarketDataResponse.ResequenceOrders(_dataResponse);
        }

        [Test]
        public void GetDefaultLowestOrder()
        {
            _dataResponse.SecurityLevel = MarketDataResponse.HighSec;
            var order = _dataResponse.LowestSellOrder(40);

            Assert.AreEqual(2.2, order.Price, "The expected order price was not returned");
            Assert.AreEqual(50, order.Quantity, "The expected order quantity was not returned");
        }

        [Test]
        public void GetDefaultHighestOrder()
        {
            _dataResponse.SecurityLevel = MarketDataResponse.HighSec;
            var order = _dataResponse.HighestBuyOrder;

            Assert.AreEqual(2.2, order.Price, "The expected order price was not returned");
            Assert.AreEqual(50, order.Quantity, "The expected order quantity was not returned");
        }

        [Test]
        public void GetBestBuyers()
        {
            _dataResponse.SecurityLevel = MarketDataResponse.HighSec;
            var orders = _dataResponse.BestBuyers;

            Assert.AreEqual(6, orders.Count, "The expected number of buy orders was not returned");
        }

        [Test]
        public void GetBestSellers()
        {
            _dataResponse.SecurityLevel = MarketDataResponse.HighSec;
            var orders = _dataResponse.BestSellers;

            Assert.AreEqual(6, orders.Count, "The expected number of sell orders was not returned");
        }
    }
}
