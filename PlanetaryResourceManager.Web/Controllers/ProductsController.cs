using PlanetaryResourceManager.Core.Helpers;
using PlanetaryResourceManager.Core.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace WebTerminal.Controllers
{
    public class ProductsController : ApiController
    {
        public IEnumerable<AnalysisItem> Get()
        {
            var productionLevel = RepositoryHelper.ProductionLevels[RepositoryHelper.RefinedMetrials];
            var analysisItems = RepositoryHelper.Repository.GetProductionItems(productionLevel);

            return analysisItems;
        }

        public IEnumerable<AnalysisItem> Get(string productLevel)
        {
            var productionLevel = RepositoryHelper.ProductionLevels[productLevel];
            var analysisItems = RepositoryHelper.Repository.GetProductionItems(productionLevel);

            return analysisItems;
        }
    }
}
