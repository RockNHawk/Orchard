using System;

namespace Rijkshuisstijl.PerformanceMonitor.Models
{
    public class PerformanceMonitorDataRecord
    {
        public PerformanceMonitorDataRecord()
        {
            //A default datetime overflows NHibernate
            HeartBeat = new DateTime(2000, 1, 1);
        }
        public virtual int Id { get; set; }
        public virtual int PerformanceMonitorRecord_Id { get; set; }
        public virtual string Count { get; set; }
        public virtual DateTime HeartBeat { get; set; }
        public virtual string Ticks { get; set; }

        public virtual bool PassedThreshold { get; set; }
        public virtual int PassedThresholdValue { get; set; }
    }
}