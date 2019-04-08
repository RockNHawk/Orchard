using System.Collections.Generic;
using System.Web.Mvc;
using MnLab.PdfVisualDesign.Binding.Drivers;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Logging;
using Orchard.UI.Admin;

namespace MnLab.PdfVisualDesign.HtmlBlocks.Controllers {
    [Admin]
    [ValidateInput(false)]
    public class AdminController : Controller {
        private readonly IOrchardServices _services;

        public ILogger Logger { get; set; }
        IContentManager _contentManager;
        public AdminController(
        IContentManager contentManager,
        IOrchardServices services

            ) {
            _services = services;
            _contentManager = contentManager;

            Logger = NullLogger.Instance;
        }

        public ActionResult GetContentValueMap(int id) {

            var content = this._contentManager.GetLatest(id);

            var contentItem = content.GetLatestVersion(_contentManager);

            var bindingDefGroups = ValueBindGridElementDriver.GetBindingDefGroups(contentItem);
            Dictionary<string, object> valueMaps = ValueBindGridElementDriver.GetValueMaps(contentItem, bindingDefGroups);

            return Json(valueMaps, JsonRequestBehavior.AllowGet);
        }

    }
}