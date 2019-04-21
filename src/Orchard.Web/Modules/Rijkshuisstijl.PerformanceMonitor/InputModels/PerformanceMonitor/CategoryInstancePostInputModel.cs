using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor
{
    public class CategoryInstancePostInputModel
    {
        public virtual string CategoryName { get; set; }

        [Required(ErrorMessage = "Instancename must be selected")]
        public virtual string InstanceName { get; set; }

        public virtual SelectList InstanceList { get; set; }
    }
}