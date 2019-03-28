using System.Collections.Generic;

namespace Orchard.Contrib.Dashboard.Models {
    public class DashboardItemContext {
        public DashboardItemContext()
        {
            Properties = new Dictionary<string, string>();
        }
        public IDictionary<string, string> Properties { get; set; }
    }
}