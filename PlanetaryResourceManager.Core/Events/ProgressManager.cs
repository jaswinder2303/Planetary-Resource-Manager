using PlanetaryResourceManager.Core.Models;

namespace PlanetaryResourceManager.Core.Events
{
    public class ProgressManager
    {
        static public event AnalysisProgressChanged OnProgressChanged;
        static public event AnalysisProgressUpdated OnProgressUpdated;

        public static void ReportProgress(int progress)
        {
            if (OnProgressChanged != null)
            {
                OnProgressChanged(progress);
            }
        }

        public static void ReportProgress(AnalysisResult progress)
        {
            if (OnProgressUpdated != null)
            {
                OnProgressUpdated(progress);
            }
        }
    }
}
