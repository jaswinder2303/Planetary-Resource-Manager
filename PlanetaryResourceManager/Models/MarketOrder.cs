using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PlanetaryResourceManager.Models
{
    class MarketOrder
    {
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Station { get; set; }
        public double Security { get; set; }
        public int MinimumVolume { get; set; }

        internal static MarketOrder Load(XElement data)
        {
            return new MarketOrder
            {
                Price = double.Parse(data.Element("price").Value),
                Quantity = int.Parse(data.Element("vol_remain").Value),
                MinimumVolume = int.Parse(data.Element("min_volume").Value),
                Security = double.Parse(data.Element("security").Value),
                Station = data.Element("station_name").Value
            };
        }
    }
}
