using System.ComponentModel.DataAnnotations;
namespace Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor
{
    public class EditInputModel
    {
        public virtual int Id { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual string InstanceName { get; set; }
        public virtual string CounterName { get; set; }
        public virtual string Duration { get; set; }
        public virtual int SampleInterval { get; set; }
        public virtual int Threshold { get; set; }
        public virtual bool ThresholdWhen { get; set; }
    }
}