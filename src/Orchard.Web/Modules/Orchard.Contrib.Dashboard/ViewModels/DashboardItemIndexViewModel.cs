using System.Collections.Generic;
using Orchard.Contrib.Dashboard.Models;

namespace Orchard.Contrib.Dashboard.ViewModels {

    public class DashboardIndexViewModel
    {
        public IList<DashboardEntry> Items { get; set; }
    }

    public class DashboardEntry
    {
        public DashboardItemContext Context { get; set; }
        public DashboardItemRecord Record { get; set; }
        public dynamic Shape { get; set; }
    }
}
