using System.Collections.Generic;
using System.Linq;
using Orchard.Contrib.Dashboard.Models;
using Orchard.Data;
using Orchard.Localization;

namespace Orchard.Contrib.Dashboard.Services
{
    public class DashboardItemsService : IDashboardItemsService {
        private readonly IRepository<DashboardItemRecord> _itemsRepo;

        public DashboardItemsService(
            IRepository<DashboardItemRecord> itemsRepo) {
            _itemsRepo = itemsRepo;
        }

        public Localizer T { get; set; }

        public DashboardItemRecord CreateItem(string name, int userid) {
            var record = new DashboardItemRecord {
                Name = name,
                UserId = userid,
                Position = _itemsRepo.Table.Count() + 1
            };
            _itemsRepo.Create(record);

            return record;
        }
        public DashboardItemRecord GetItem(int id)
        {
            return _itemsRepo.Get(id);
        }

        public void DeleteItem(int eventId)
        {
            var e = _itemsRepo.Get(eventId);
            if (e != null)
            {
                _itemsRepo.Delete(e);
            }
        }

        public IEnumerable<DashboardItemRecord> GetUserItems(int userId)
        {
            return _itemsRepo.Table
                .Where(i=>i.UserId==userId||i.UserId<=0)
                .OrderBy(i=>i.Position)
                .ToList();
        }
        public IEnumerable<DashboardItemRecord> GetAllItems()
        {
            return _itemsRepo.Table.ToList();
        }

        public void MoveUp(int itemId)
        {
            var item = _itemsRepo.Get(itemId);

            // look for the previous action in order in same rule
            var previous = _itemsRepo.Table
                .Where(x => x.Position < item.Position && (x.UserId == item.UserId || x.UserId<0))
                .OrderByDescending(x => x.Position)
                .FirstOrDefault();

            // nothing to do if already at the top
            if (previous == null)
            {
                return;
            }

            // switch positions
            var temp = previous.Position;
            previous.Position = item.Position;
            item.Position = temp;
        }

        public void MoveDown(int itemId)
        {
            var item = _itemsRepo.Get(itemId);

            // look for the next action in order in same rule
            var next = _itemsRepo.Table
                .Where(x => x.Position > item.Position && (x.UserId == item.UserId || x.UserId < 0))
                .OrderBy(x => x.Position)
                .FirstOrDefault();

            // nothing to do if already at the end
            if (next == null)
            {
                return;
            }

            // switch positions
            var temp = next.Position;
            next.Position = item.Position;
            item.Position = temp;
        }
    }
}