using PlanetaryResourceManager.Api.Models;
using PlanetaryResourceManager.Core.Helpers;
using PlanetaryResourceManager.Core.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace PlanetaryResourceManager.Api.Controllers
{

    public class ProductsController : ApiController
    {
        [HttpGet]
        public IEnumerable<AnalysisItem> GetAllProducts()
        {
            var productionLevel = RepositoryHelper.ProductionLevels[RepositoryHelper.RefinedMaterials];
            var analysisItems = RepositoryHelper.Repository.GetProductionItems(productionLevel);

            return analysisItems;
        }

        [HttpGet]
        public IEnumerable<AnalysisItem> LoadAllProducts(string id)
        {
            var productionLevel = RepositoryHelper.ProductionLevels[id];
            var analysisItems = RepositoryHelper.Repository.GetProductionItems(productionLevel);

            return analysisItems;
        }

        [HttpGet]
        public IEnumerable<ProductType> LoadProductLevels()
        {
            var productTypes = new List<ProductType>();

            foreach (var level in RepositoryHelper.ProductionLevels)
            {
                productTypes.Add(new ProductType
                {
                    Name = level.Key,
                    Level = level.Value
                });
            }

            return productTypes;
        }
    }
}
