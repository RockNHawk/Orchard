using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Contrib.Dashboard.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;

namespace Orchard.Contrib.Dashboard.Services
{
    public interface IDashboardItemsManager : IDependency {
        IEnumerable<TypeDescriptor<DashboardItemDescriptor>> DescribeItems();
    }

    public class DashboardItemsManager : IDashboardItemsManager {
        private readonly IEnumerable<IDashboardItemProvider> _itemProviders;
        private readonly dynamic _shapeFactory;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public DashboardItemsManager(
            IEnumerable<IDashboardItemProvider> itemProviders,
            IShapeFactory shapeFactory
            ) 
        {
            _itemProviders = itemProviders;
            _shapeFactory = shapeFactory;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }
        
        public IEnumerable<TypeDescriptor<DashboardItemDescriptor>> DescribeItems()
        {
            var context = new DescribeDashboardItemContext();
            foreach (var provider in _itemProviders)
            {
                provider.Describe(context);
            }
            return context.Describe();
        }
    }
}