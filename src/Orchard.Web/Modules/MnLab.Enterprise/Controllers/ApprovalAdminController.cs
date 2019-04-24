using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MnLab.Enterprise.Approval.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Localization.Services;
using MnLab.Enterprise.Approval;
using Orchard;
using Orchard.Logging;

namespace MnLab.Enterprise.Controllers {
    [Admin]
    public class ApprovalAdminController : Controller, IUpdateModel {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly ICultureManager _cultureManager;
        private readonly ICultureFilter _cultureFilter;



        readonly ContentApprovalService _approvalService;


        public ApprovalAdminController(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            ICultureManager cultureManager,
            ICultureFilter cultureFilter,
          ContentApprovalService approvalService
            ) {
            Services = orchardServices;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            _cultureManager = cultureManager;
            _cultureFilter = cultureFilter;
            _approvalService = approvalService;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }
        dynamic Shape { get; set; }
        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public ActionResult ApprovalList(PagerParameters pagerParameters, ApprovalPartRecord part) {

            // Create a basic query that selects all customer content items, joined with the UserPartRecord table
            var removedItems = Services.ContentManager.Query().List();
            

            return View(removedItems);
        }

        public ActionResult Edit(int id) {
            var customer = _approvalService.GetContent(id);
            var model = _contentManager.BuildEditor(customer);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            var customer = _approvalService.GetContent(id);
            var model = _contentManager.UpdateEditor(customer, this);

            if (!ModelState.IsValid)
                return View(model);

            //_notifier.Add(NotifyType.Information, T("Your customer has been saved"));
            return RedirectToAction("Edit", new { id });
        }

        public ActionResult ListAddresses(int id) {
            //var customer = _approvalService.Approve;
            //return View(addresses);
            return null;
        }

        public ActionResult ListOrders(int id) {
            return null;
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.Text);
        }
    }
}