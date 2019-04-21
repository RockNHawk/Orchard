using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor
{
    public class ShowInputModel
    {
        public virtual int PerformanceMonitorRecord_Id { get; set; }
        public virtual bool AutoRefresh { get; set; }
    }
}