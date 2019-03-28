using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Contrib.Dashboard.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;

namespace Orchard.Contrib.Dashboard.Services {
    public class DescribeDashboardItemContext
    {
        private readonly Dictionary<string, DescribeDashboardItemFor> _describes = new Dictionary<string, DescribeDashboardItemFor>();

        public IEnumerable<TypeDescriptor<DashboardItemDescriptor>> Describe()
        {
            return _describes.Select(kp => new TypeDescriptor<DashboardItemDescriptor>
            {
                Category = kp.Key,
                Name = kp.Value.Name,
                Description = kp.Value.Description,
                Descriptors = kp.Value.Types
            });
        }
        public DescribeDashboardItemFor For(string category)
        {
            return For(category, null, null);
        }

        public DescribeDashboardItemFor For(string category, LocalizedString name, LocalizedString description)
        {
            DescribeDashboardItemFor describeFor;
            if (!_describes.TryGetValue(category, out describeFor))
            {
                describeFor = new DescribeDashboardItemFor(category, name, description);
                _describes[category] = describeFor;
            }
            return describeFor;
        }

    }
}