#region Usings

using System.Collections.Generic;
using Rijkshuisstijl.PerformanceMonitor.Models;
using Rijkshuisstijl.PerformanceMonitor.WorkerServices;

#endregion

namespace Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor
{
    public class IndexViewModel
    {
        public List<PerformanceMonitorRecord> PerformanceMonitorRecords { get; set; }
    }
}