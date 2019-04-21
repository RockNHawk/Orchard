using Orchard;
using Rijkshuisstijl.PerformanceMonitor.Models;
using System.Collections.Generic;

namespace Rijkshuisstijl.PerformanceMonitor.Services
{
    public interface IPerformanceCounterService : IDependency
    {
        string GetPerformanceCounterReading(string categoryName, string instanceName, string counterName);
    }
}