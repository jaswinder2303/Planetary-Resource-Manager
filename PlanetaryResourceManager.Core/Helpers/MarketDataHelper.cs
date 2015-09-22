using PlanetaryResourceManager.Core.Models;
using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace PlanetaryResourceManager.Core.Helpers
{
    public class MarketDataHelper : IDisposable
    {
        private WebClient _client;
        private string _host;
        public const string Jita = "30000142";
        public const string Freshness = "24";
        public const string QuickLook = "http://api.eve-central.com/api/quicklook";

        public MarketDataHelper(string host)
        {
            _client = new WebClient();
            _client.Headers.Add("User-Agent", "Market-Analysis");
            _host = host;
        }

        public MarketDataResponse GetData(MarketDataRequest request)
        {
            _client.QueryString.Clear();
            _client.QueryString.Add("typeid", request.TypeId);
            _client.QueryString.Add("sethours", request.Duration);

            if (request.SystemId != null)
            {
                _client.QueryString.Add("usesystem", request.SystemId);
            }

            if (request.MinimumQuantity != null)
            {
                _client.QueryString.Add("setminQ", request.MinimumQuantity);
            }

            string result = _client.DownloadString(_host);

            XDocument data = XDocument.Parse(result);

            var responseData = (from item in data.Root.Descendants("quicklook")
                                let sellOrders = item.Descendants("sell_orders").Descendants("order").OrderBy(arg => arg.Element("price").Value)
                                let buyOrders = item.Descendants("buy_orders").Descendants("order").OrderByDescending(arg => arg.Element("price").Value)
                                select new MarketDataResponse
                                {
                                    Commodity = item.Element("itemname").Value,
                                    BuyOrders = buyOrders.Select(order => MarketOrder.Load(order)).ToList(),
                                    SellOrders = sellOrders.Select(order => MarketOrder.Load(order)).ToList()
                                }).FirstOrDefault();

            return responseData;
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
