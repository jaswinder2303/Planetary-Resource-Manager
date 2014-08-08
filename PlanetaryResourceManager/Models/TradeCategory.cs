using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class TradeCategory
    {
        public string Name { get; set; }
        public List<TradeGroup> Groups { get; set; }
        public int TradeScore { get; set; }
        public DateTime? LastScanDate { get; set; }
    }
}
