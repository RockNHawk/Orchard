using System.Collections.Generic;
using Orchard.Contrib.Dashboard.Models;

namespace Orchard.Contrib.Dashboard.ViewModels {

    public class DashboardItemIndexViewModel
    {
        public IList<DashboardItemEntry> Items { get; set; }
        public DashboardItemIndexOptions Options { get; set; }
        public dynamic Pager { get; set; }
    }

    public class DashboardItemEntry
    {
        public DashboardItemRecord Record { get; set; }
        public bool IsChecked { get; set; }
        public int Id { get; set; }
    }

    public class DashboardItemIndexOptions
    {
        public string Search { get; set; }
        public DashboardItemOrder Order { get; set; }
        public DashboardItemFilter Filter { get; set; }
        public DashboardItemBulkAction BulkAction { get; set; }
    }

    public enum DashboardItemOrder
    {
        Position,
        Type,
        Category
    }

    public enum DashboardItemFilter
    {
        All,
        Enabled,
        Disabled
    }

    public enum DashboardItemBulkAction
    {
        None,
        Enable,
        Disable,
        Delete
    }
}
