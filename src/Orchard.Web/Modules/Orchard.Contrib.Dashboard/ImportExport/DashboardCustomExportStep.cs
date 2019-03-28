using System.Collections.Generic;
using Orchard.Events;

namespace Orchard.Contrib.Dashboard.ImportExport {
    public interface ICustomExportStep : IEventHandler {
        void Register(IList<string> steps);
    }

    public class DashboardCustomExportStep : ICustomExportStep {
        public void Register(IList<string> steps) {
            steps.Add("Dashboard");
        }
    }
}