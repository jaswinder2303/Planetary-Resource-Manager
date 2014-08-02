using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class AnalysisItem
    {
        public Product Product { get; set; }
        public List<RawMaterial> Materials { get; set; }
        public int ProductionLevel { get; set; }
    }
}
