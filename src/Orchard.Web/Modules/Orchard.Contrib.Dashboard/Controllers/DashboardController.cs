using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Contrib.Dashboard.Models;
using Orchard.Contrib.Dashboard.Services;
using Orchard.Contrib.Dashboard.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.UI.Admin;

namespace Orchard.Contrib.Dashboard.Controllers {
    [Admin]
    public class DashboardController : Controller {

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public dynamic Shape { get; set; }
        private readonly IDashboardItemsManager _dashboardItemsManager;
        private readonly IDashboardItemsService _dashboardItemsService;

        public DashboardController(
            IOrchardServices services,
            IFormManager formManager,
            IShapeFactory shapeFactory,
            IDashboardItemsManager dashboardItemsManager,
            IDashboardItemsService dashboardItemsService) {
            _dashboardItemsManager = dashboardItemsManager;
            _dashboardItemsService = dashboardItemsService;
            Services = services;
            Shape = shapeFactory;
        }



        public ActionResult Index() {

            var records = _dashboardItemsService.GetUserItems(Services.WorkContext.CurrentUser.Id);
            var describers = _dashboardItemsManager.DescribeItems();

            var tuples = records
                .Where(r=>r.Enabled)
                .Select(r => new {r, d = FindDescribtor(r, describers)})
                .Where(t => t.d != null)
                .Select(t => new DashboardEntry {
                    Record = t.r,
                    Shape = t.d.Display(new DashboardItemContext {Properties = FormParametersHelper.FromString(t.r.Parameters) })
                })
                .ToList();

            var viewModel = new DashboardIndexViewModel();
            viewModel.Items = tuples;

            return View(viewModel);
        }

        private DashboardItemDescriptor FindDescribtor(DashboardItemRecord record, IEnumerable<TypeDescriptor<DashboardItemDescriptor>> describers) {
            var typeDescriber = describers.FirstOrDefault(d => d.Category == record.Category);
            if(typeDescriber!=null) return typeDescriber.Descriptors.FirstOrDefault(d => d.Type == record.Type);
            return null;
        }
    }
}