using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Models
{
    public class RawMaterial : Commodity
    {
        public RawMaterial()
        {
            IsRawMaterial = true;
        }

        public double ImportCost { get; set; }
        public int InputLevel { get; set; }
        public int InputBatchSize { get; set; }
    }
}
