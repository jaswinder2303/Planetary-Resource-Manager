using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class InputComparer : IEqualityComparer<RawMaterial>
    {
        public bool Equals(RawMaterial x, RawMaterial y)
        {
            return x.InputLevel == y.InputLevel;
        }

        public int GetHashCode(RawMaterial obj)
        {
            return obj.InputLevel.GetHashCode();
        }
    }
}
