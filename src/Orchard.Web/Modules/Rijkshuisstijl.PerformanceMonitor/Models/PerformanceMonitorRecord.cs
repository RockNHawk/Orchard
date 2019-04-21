using System;

namespace Rijkshuisstijl.PerformanceMonitor.Models
{
    public class PerformanceMonitorRecord
    {
        public virtual int Id { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual string InstanceName { get; set; }
        public virtual string CounterName { get; set; }
        public virtual string NodeName { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual string Duration { get; set; }
        public virtual int SampleInterval { get; set; }
        public virtual int Threshold { get; set; }
        public virtual bool ThresholdWhen { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string InitialValue { get; set; }
        public virtual string LastValue { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual string DurationCount { get; set; }
        public virtual double Mean { get; set; }
        public virtual double Minimum { get; set; }
        public virtual double Maximum { get; set; }    
    }
}