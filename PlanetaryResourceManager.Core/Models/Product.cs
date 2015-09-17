using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Models
{
    public class Product : Commodity
    {
        public int OutputBatchSize { get; set; }
        public double ExportCost { get; set; }
    }
}
