using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Events
{
    class ProgressManager
    {
        static internal event AnalysisProgressChanged OnProgressChanged;

        internal static void ReportProgress(int progress)
        {
            if (OnProgressChanged != null)
            {
                OnProgressChanged(progress);
            }
        }
    }
}
