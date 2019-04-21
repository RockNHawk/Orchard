using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor
{
    public class CategoryInstanceInputModel
    {
        public virtual string CategoryName { get; set; }
        public virtual string InstanceName { get; set; }
        public virtual SelectList InstanceList { get; set; }
    }
}