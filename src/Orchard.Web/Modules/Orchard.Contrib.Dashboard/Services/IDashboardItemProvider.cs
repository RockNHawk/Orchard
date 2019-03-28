using System.Web.Mvc;
using Orchard.Events;

namespace Orchard.Contrib.Dashboard.Services 
{
    public interface IDashboardItemProvider : IEventHandler 
    {
        void Describe(DescribeDashboardItemContext context);
    }
}