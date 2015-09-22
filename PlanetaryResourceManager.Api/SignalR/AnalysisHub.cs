using Microsoft.AspNet.SignalR;
using PlanetaryResourceManager.Api.Services;
using PlanetaryResourceManager.Core.Events;
using PlanetaryResourceManager.Core.Models;

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
            service.Start(level);
        }

        private void OnAnalysisProgressUpdated(AnalysisResult progress)
        {
            Clients.All.updateAnalysisItem(progress);

            if (progress.ProgressIndex == 100)
            {
                RebuildList();
            }
        }

        private void RebuildList()
        {
            //rebuild the list and send
            Clients.All.analysisComplete("Analysis complete");
            ProgressManager.OnProgressUpdated -= OnAnalysisProgressUpdated;
        }
    }
}