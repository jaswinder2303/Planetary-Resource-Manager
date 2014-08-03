using PlanetaryResourceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Helpers
{
    class ProductionHelper
    {
        internal const int BatchSize = 8000;

        public static ProductionResult Calculate(Product product, List<RawMaterial> materials, int batchSize = BatchSize)
        {
            if (materials.Count != 2)
            {
                throw new InvalidOperationException("Two raw materials needed for every product");
            }

            ProductionResult result = new ProductionResult();
            result.InputQuantity = product.InputBatchSize * batchSize;
            result.OutputQuantity = product.OutputBatchSize * batchSize;
            result.SaleCost = product.Price * result.OutputQuantity;
            result.Expenses = (result.InputQuantity * materials[0].ImportCost) + (result.InputQuantity * materials[1].ImportCost) + (result.OutputQuantity * product.ExportCost);
            result.PurchaseCost = (materials[0].Price * result.InputQuantity) + (materials[1].Price * result.InputQuantity);
            result.ProfitMargin = result.SaleCost - (result.PurchaseCost + result.Expenses);

            return result;
        }

        public static int GetInputBatchSize(int productionLevel)
        {
            switch (productionLevel)
            {
                case 2:
                    return 3000;
                case 3:
                    return 40;
                default:
                    return 0;
            }
        }

        public static int GetOutputBatchSize(int productionLevel)
        {
            switch (productionLevel)
            {
                case 2:
                    return 20;
                case 3:
                    return 5;
                default:
                    return 1;
            }
        }

        public static double GetImportCost(int productionLevel)
        {
            switch (productionLevel)
            {
                case 2:
                    return 0.38;
                case 3:
                    return 30;
                default:
                    return 0;
            }
        }

        public static double GetExportCost(int productionLevel)
        {
            switch (productionLevel)
            {
                case 2:
                    return 60;
                case 3:
                    return 1080;
                default:
                    return 0.75;
            }
        }
    }
}
