using System.Collections.Generic;
using Orchard.Contrib.Dashboard.Models;

namespace Orchard.Contrib.Dashboard.Services {
    public interface IDashboardItemsService : IDependency {
        DashboardItemRecord CreateItem(string name, int userid = -1);
        DashboardItemRecord GetItem(int id);
        void DeleteItem(int eventId);
        void MoveUp(int itemId);
        void MoveDown(int itemId);

        IEnumerable<DashboardItemRecord> GetUserItems(int userId);
        IEnumerable<DashboardItemRecord> GetAllItems();
    }
}