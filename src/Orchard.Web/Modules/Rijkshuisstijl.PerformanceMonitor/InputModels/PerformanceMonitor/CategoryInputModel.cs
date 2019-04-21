using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Rijkshuisstijl.PerformanceMonitor.InputModels.PerformanceMonitor
{
    public class CategoryInputModel
    {
        public virtual string CategoryName { get; set; }
        public virtual SelectList CategoryList { get; set; }
    }
}