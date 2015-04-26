using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    public class RawMaterial : Commodity
    {
        public RawMaterial()
        {
            IsRawMaterial = true;
        }

        public double ImportCost { get; set; }
    }
}
