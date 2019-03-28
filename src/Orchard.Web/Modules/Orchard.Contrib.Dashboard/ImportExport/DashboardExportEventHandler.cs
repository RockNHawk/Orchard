using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Orchard.Contrib.Dashboard.Services;
using Orchard.Events;

namespace Orchard.Contrib.Dashboard.ImportExport {
    public interface IExportEventHandler : IEventHandler {
        void Exporting(dynamic context);
        void Exported(dynamic context);
    }

    public class DashboardExportHandler : IExportEventHandler {
        private readonly IDashboardItemsService _dashboardItemsServices;

        public DashboardExportHandler(IDashboardItemsService dashboardItemsServices)
        {
            _dashboardItemsServices = dashboardItemsServices;
        }

        public void Exporting(dynamic context) {
        }

        public void Exported(dynamic context) {

            if (!((IEnumerable<string>)context.ExportOptions.CustomSteps).Contains("Dashboard"))
            {
                return;
            }

            var allItems = _dashboardItemsServices.GetAllItems().ToList();
            
            if(!allItems.Any()) {
                return;
            }

            var root = new XElement("Dashboard");
            context.Document.Element("Orchard").Add(root);

            foreach(var record in allItems) {
                root.Add(new XElement("Item",
                    new XAttribute("Name", record.Name),
                    new XAttribute("Enabled", record.Enabled.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("Position", record.Position),
                    new XAttribute("Type", record.Type ?? string.Empty),
                    new XAttribute("Category", record.Category ?? string.Empty),
                    new XAttribute("Parameters", record.Parameters ?? string.Empty)
                ));
            }
        }
    }
}

