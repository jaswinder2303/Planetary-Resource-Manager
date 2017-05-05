using System.Xml.Linq;

namespace PlanetaryResourceManager.Core.Models
{
    public class MarketOrder
    {
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Station { get; set; }
        public double Security { get; set; }
        public int MinimumVolume { get; set; }
        public string ReportedDate { get; set; }

        internal static MarketOrder Load(XElement data)
        {
            return new MarketOrder
            {
                Price = double.Parse(Extract(data, "price", "0.0")),
                Quantity = int.Parse(Extract(data, "vol_remain", "0")),
                MinimumVolume = int.Parse(Extract(data, "min_volume", "1")),
                Security = double.Parse(Extract(data, "security", "-2.0")),
                Station = Extract(data, "station_name", "None"),
                ReportedDate = Extract(data, "reported_time", "01/01/1901")
            };
        }

        private static string Extract(XContainer data, string value, string defaultValue)
        {
            var element = data.Element(value);
            return element?.Value ?? defaultValue;
        }
    }
}
