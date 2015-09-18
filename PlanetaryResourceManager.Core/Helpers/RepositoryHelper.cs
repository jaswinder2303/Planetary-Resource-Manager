using PlanetaryResourceManager.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Helpers
{
    public class RepositoryHelper
    {
        private static Dictionary<string, int> _productionLevels;
        private static EveRepository _repository;
        public static string RawMaterials = "Raw Materials";
        public static string ProcessedMaterials = "Processed Materials";
        public static string RefinedMaterials = "Refined Materials";
        public static string SpecializedMaterials = "Specialized Materials";
        public static string AdvancedMaterials = "Advanced Materials";

        static RepositoryHelper()
        {
            _repository = new EveRepository();
            _productionLevels = new Dictionary<string, int>{
                {RawMaterials, 1},
                {ProcessedMaterials, 2},
                {RefinedMaterials, 3},
                {SpecializedMaterials, 4},
                {AdvancedMaterials, 5}
            };
        }

        public static EveRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        public static Dictionary<string, int> ProductionLevels
        {
            get
            {
                return _productionLevels;
            }
        }
    }
}
