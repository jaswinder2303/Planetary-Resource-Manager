using Microsoft.AspNet.SignalR;
using PlanetaryResourceManager.Api.Services;
using PlanetaryResourceManager.Core.Events;
using PlanetaryResourceManager.Core.Helpers;
using PlanetaryResourceManager.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlanetaryResourceManager.Api.SignalR
{
    public class AnalysisHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Start(string level)
        {
            ProgressManager.OnProgressUpdated += OnAnalysisProgressUpdated;
            AnalysisService service = new AnalysisService();

            var productionLevel = RepositoryHelper.ProductionLevels[level];
            AnalysisItems = RepositoryHelper.Repository.GetProductionItems(productionLevel);

            service.Start(productionLevel, AnalysisItems, RebuildList);
        }

        private List<AnalysisItem> AnalysisItems { get; set; }

        private void OnAnalysisProgressUpdated(AnalysisResult progress)
        {
            Clients.All.updateAnalysisItem(progress);

            //if (progress.ProgressIndex == 100)
            //{
            //    RebuildList();
            //}
        }

        private void RebuildList(List<AnalysisItem> analysisItems)
        {
            Clients.All.analysisComplete("Complete");
            ProgressManager.OnProgressUpdated -= OnAnalysisProgressUpdated;
        }
    }
}