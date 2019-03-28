using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Contrib.Dashboard.Models;
using Orchard.Contrib.Dashboard.Services;

namespace Orchard.Contrib.Dashboard.ViewModels {

    public class DashboardItemAddViewModel {
        //[Required, StringLength(64)]
        //public string Name { get; set; }
        public IEnumerable<TypeDescriptor<DashboardItemDescriptor>> Items { get; set; }
    }
}
