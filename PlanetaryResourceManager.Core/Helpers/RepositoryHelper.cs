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
        public static string RawMetrials = "Raw Metrials";
        public static string ProcessedMetrials = "Processed Metrials";
        public static string RefinedMetrials = "Refined Metrials";
        public static string SpecializedMetrials = "Specialized Metrials";
        public static string AdvancedMetrials = "Advanced Metrials";

        static RepositoryHelper()
        {
            _repository = new EveRepository();
            _productionLevels = new Dictionary<string, int>{
                {RawMetrials, 1},
                {ProcessedMetrials, 2},
                {RefinedMetrials, 3},
                {SpecializedMetrials, 4},
                {AdvancedMetrials, 5}
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
