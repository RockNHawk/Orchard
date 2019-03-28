using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Contrib.Dashboard.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;

namespace Orchard.Contrib.Dashboard.Services
{
    public class DescribeDashboardItemFor
    {
        private readonly string _category;

        public DescribeDashboardItemFor(string category, LocalizedString name, LocalizedString description)
        {
            Types = new List<DashboardItemDescriptor>();
            _category = category;
            Name = name;
            Description = description;
        }
        public LocalizedString Name { get; private set; }
        public LocalizedString Description { get; private set; }
        public List<DashboardItemDescriptor> Types { get; private set; }

        public DescribeDashboardItemFor Item(string type, LocalizedString name, LocalizedString description, Func<DashboardItemContext, dynamic> display, string form = null)
        {
            Types.Add(new DashboardItemDescriptor {
                Type = type, 
                Name = name, 
                Description = description, 
                Category = _category, 
                Display = display,
                Form = form
            });
            return this;
        }
    }
}