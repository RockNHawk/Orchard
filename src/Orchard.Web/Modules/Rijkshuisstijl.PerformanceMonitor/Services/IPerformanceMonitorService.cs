using Orchard;
using Rijkshuisstijl.PerformanceMonitor.Models;
using System.Collections.Generic;

namespace Rijkshuisstijl.PerformanceMonitor.Services
{
    public interface IPerformanceMonitorService : IDependency
    {
        void HeartBeat();
        List<string> CategoryNames(out string ex);
        List<string> GetInstanceNames(string categoryName);
        List<string> GetCounterNames(string categoryName, string instanceName);
        bool GetInstanceType(string categoryName);
        bool CheckThreshold(int thresholdValue, bool thresholdWhen, double countervalue, ref int passedThresholdValue);
    }
}