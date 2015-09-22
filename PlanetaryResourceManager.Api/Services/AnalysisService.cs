using PlanetaryResourceManager.Api.Models;
using PlanetaryResourceManager.Core.Events;
using PlanetaryResourceManager.Core.Helpers;
using PlanetaryResourceManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PlanetaryResourceManager.Api.Services
{
    public class AnalysisService
    {
        private int ProductionLevel { get; set; }
        private List<AnalysisItem> AnalysisItems { get; set; }

        public async void Start(int level, List<AnalysisItem> analysisItems, Action<List<AnalysisItem>> postFunc)
        {
            ProductionLevel = level;
            AnalysisItems = analysisItems;

            var progress = new Progress<AnalysisResult>(result =>
            {
                ProgressManager.ReportProgress(result);
            });

            await Task.Factory.StartNew(() => Analyze(progress)).ContinueWith((task) =>
            {
                AnalysisItems = AnalysisItems.OrderByDescending(member => member.ProfitMargin).ToList();
                postFunc(AnalysisItems);
            });
        }

        private void Analyze(IProgress<AnalysisResult> progress)
        {
            int index = 0;
            var itemTasks = new List<Task>();

            foreach (var item in AnalysisItems)
            {
                itemTasks.Add(Task.Factory.StartNew(() =>
                {
                    using (MarketDataHelper helper = new MarketDataHelper(MarketDataHelper.QuickLook))
                    {
                        MarketDataRequest request = new MarketDataRequest
                        {
                            TypeId = item.Product.ItemId.ToString(),
                            SystemId = MarketDataHelper.Jita,
                            Duration = MarketDataHelper.Freshness
                        };

                        var productData = helper.GetData(request);
                        MarketDataResponse.ResequenceOrders(productData);
                        var order = productData.HighestBuyOrder;
                        item.Product.Price = order != null ? order.Price : 0.0;
                        item.Product.ExportCost = ProductionHelper.GetExportCost(ProductionLevel);
                        item.Product.OutputBatchSize = ProductionHelper.GetOutputBatchSize(ProductionLevel);
                        item.Product.Data = productData;

                        foreach (var input in item.Materials)
                        {
                            request = new MarketDataRequest
                            {
                                TypeId = input.ItemId.ToString(),
                                Duration = MarketDataHelper.Freshness,
                                SystemId = MarketDataHelper.Jita
                            };

                            var materialData = helper.GetData(request);
                            MarketDataResponse.ResequenceOrders(materialData);
                            //order = materialData.LowestSellOrder(AnalysisViewModel.MinimumQuanity);
                            order = materialData.LowestSellOrder(null);
                            input.Price = order != null ? order.Price : 0.0;
                            input.ImportCost = ProductionHelper.GetImportCost(input.InputLevel);
                            input.InputBatchSize = ProductionHelper.GetInputBatchSize(input.InputLevel);
                            input.Data = materialData;
                        }

                        var productionResult = ProductionHelper.Calculate(item.Product, item.Materials);
                        item.ProductionCost = productionResult.PurchaseCost;
                        item.SaleValue = productionResult.SaleCost;
                        item.ProfitMargin = productionResult.ProfitMargin;
                        item.UpdateProperties();

                        var currentProgress = ((double)++index / AnalysisItems.Count) * 100;

                        progress.Report(new AnalysisResult
                        {
                            ProgressIndex = (int)currentProgress,
                            Item = item
                        });
                    }
                }, TaskCreationOptions.AttachedToParent));
            }

            Task.Factory.ContinueWhenAll(itemTasks.ToArray(), groupedTasks => {
            });
        }
    }
}