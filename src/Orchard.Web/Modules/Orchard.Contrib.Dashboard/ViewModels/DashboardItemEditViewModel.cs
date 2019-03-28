using Orchard.Contrib.Dashboard.Services;

namespace Orchard.Contrib.Dashboard.ViewModels {

    public class DashboardItemEditViewModel {
        public int Id { get; set; }
        public DashboardItemDescriptor Item { get; set; }
        public dynamic Form { get; set; }
    }
}
