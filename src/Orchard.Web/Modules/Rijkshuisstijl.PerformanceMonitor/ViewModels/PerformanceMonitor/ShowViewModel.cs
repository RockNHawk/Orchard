
using System.Collections.Generic;
using Rijkshuisstijl.PerformanceMonitor.Models;
using Rijkshuisstijl.PerformanceMonitor.WorkerServices;

namespace Rijkshuisstijl.PerformanceMonitor.ViewModels.PerformanceMonitor
{
    public class ShowViewModel
    {
        public List<PerformanceMonitorDataRecord> PerformanceMonitorDataRecords { get; set; }
        public int PerformanceMonitorRecord_Id { get; set; }
        public string LabelCounter { get; set; }
        public int Threshold { get; set; }
        public bool ThresholdWhen { get; set; }
        public int SampleInterval { get; set; }

        public bool AutoRefresh { get; set; }

        public double MeanValue { get; set; }
        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }  

        public int id { get; set; }
        public string LabelAxisY { get; set; }
        public string LineName { get; set; }
        public List<PlotDataViewModel> data { get; set; }
    }

    public class PlotDataViewModel
    {
        public string Ticks;
        public double Value { get; set; }
    }
}