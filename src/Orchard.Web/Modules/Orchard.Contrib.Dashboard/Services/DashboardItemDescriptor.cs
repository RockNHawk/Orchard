using System;
using Orchard.Contrib.Dashboard.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;

namespace Orchard.Contrib.Dashboard.Services {
    public class DashboardItemDescriptor
    {
        public Func<DashboardItemContext, dynamic> Display { get; set; }
        public string Category { get; set; } // e.g. Content
        public string Type { get; set; } // e.g. Created
        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }
        public string Form { get; set; }
    }
}