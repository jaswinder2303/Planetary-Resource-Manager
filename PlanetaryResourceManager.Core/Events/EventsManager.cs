using System.Windows;
using PlanetaryResourceManager.Core.Models;

namespace PlanetaryResourceManager.Core.Events
{
    public delegate void AnalysisProgressChanged(int progress);
    public delegate void AnalysisProgressUpdated(AnalysisResult progress);
    public delegate void ListOrderRequested(DependencyObject sender);

    class EventsManager
    {
        
    }
}
