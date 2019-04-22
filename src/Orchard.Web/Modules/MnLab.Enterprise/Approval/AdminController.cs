using System.Collections.Generic;
using System.Web.Mvc;
//using MnLab.PdfVisualDesign.Binding.Drivers;
using Orchard;
using Orchard.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Contents.Controllers;
using Orchard.Logging;
using Orchard.UI.Admin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Core.Contents.ViewModels;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Settings;
using Orchard.Utility.Extensions;
using Orchard.Localization.Services;
using Orchard.Core.Contents;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using Orchard.Users.Models;
using Rhythm.Web;

namespace MnLab.Enterprise.Approval.Controllers {
    //[Admin]
    [ValidateInput(false)]
    public class AdminController : RhythmControllerBase {


        //public ILogger Logger { get; set; }

        //IContentManager _contentManager;
        //private readonly IOrchardServices _services;

        //public AdminController(
        //IContentManager contentManager,
        //IOrchardServices services

        //    ) {
        //    _services = services;
        //    _contentManager = contentManager;

        //    Logger = NullLogger.Instance;
        //}



        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly ICultureManager _cultureManager;
        private readonly ICultureFilter _cultureFilter;



        readonly ContentApprovalService _approvalService;


        public AdminController(
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


        //[HttpPost, ActionName("Create")]
        //[Orchard.Mvc.FormValueRequired("submit.Save")]
        //public ActionResult CreatePOST(string id, string returnUrl) {
        //    return CreatePOST(id, returnUrl, contentItem => {
        //        if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
        //            _contentManager.Publish(contentItem);
        //    });
        //}


        public ActionResult Approve(int id, string AuditOpinion, string returnUrl) {
            var workContext = base.ControllerContext.GetWorkContext();
            //var user = workContext.CurrentUser.As<UserPart>().Record;
            _approvalService.Approve(workContext, id, AuditOpinion);
            return Done(T("Approved").Text);
        }


        public ActionResult Reject(int id, string AuditOpinion, string returnUrl) {
            var workContext = base.ControllerContext.GetWorkContext();
            //  var user = workContext.CurrentUser.As<UserPart>().Record;
            _approvalService.Reject(workContext, id, AuditOpinion);
            return Done(T("Rejected").Text);
        }

        [ActionName("Create")]
        [Orchard.Mvc.FormValueRequired("submit.Commit")]
        public ActionResult Create(string id, string returnUrl) {

            //// pass a dummy content to the authorization check to check for "own" variations
            //var dummyContent = _contentManager.New(id);

            //if (!Services.Authorizer.Authorize(Permissions.PublishContent, dummyContent, T("Couldn't create content")))
            //    return new HttpUnauthorizedResult();

            return CreatePOST(id, returnUrl, contentItem => {
                var workContext = base.ControllerContext.GetWorkContext();
                var user = workContext.CurrentUser.As<UserPart>().Record;
                _approvalService.CommmitApproval(workContext, new CreateApprovalCommand {
                    ContentItem = contentItem,
                    CommitBy = user,
                    ApprovalType = ApprovalType.Creation,
                });
            });
        }

        [ActionName("Edit")]
        [Orchard.Mvc.FormValueRequired("submit.Commit")]
        public ActionResult Edit(int id, string returnUrl) {

            return EditPOST(id, returnUrl, contentItem => {
                //var approvalSupportPart = contentItem.As<ApprovalSupportPart>();
                var workContext = base.ControllerContext.GetWorkContext();
                var user = workContext.CurrentUser.As<UserPart>().Record;
                _approvalService.CommmitApproval(workContext, new CreateApprovalCommand {
                    ContentItem = contentItem,
                    CommitBy = user,
                    ApprovalType = ApprovalType.Modification,
                });
                //if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                //    _contentManager.Publish(contentItem);

                //approvalSupportPart.ApprovalType

            });
        }


        private ActionResult CreatePOST(string id, string returnUrl, Action<ContentItem> callback) {

            var contentItem = _contentManager.New(id);

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            _contentManager.Create(contentItem, VersionOptions.Draft);

            var model = _contentManager.UpdateEditor(contentItem, this);

            if (!ModelState.IsValid) {
                _transactionManager.Cancel();
                //return View(model);
                return Fail();
            }

            callback(contentItem);

            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been created.")
                : T("Your {0} has been created.", contentItem.TypeDefinition.DisplayName));
            if (!string.IsNullOrEmpty(returnUrl)) {
                // return this.RedirectLocal(returnUrl);
                return Done(T("Commited").Text).RedirectUrl(returnUrl);
            }
            var adminRouteValues = _contentManager.GetItemMetadata(contentItem).AdminRouteValues;
             //return RedirectToRoute(adminRouteValues);
            return Done(T("Commited").Text).RedirectUrl(Url.RouteUrl(adminRouteValues));
        }


        private ActionResult EditPOST(int id, string returnUrl, Action<ContentItem> callback) {
            var contentItem = _contentManager.Get(id, VersionOptions.DraftRequired);

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.EditContent, contentItem, T("Couldn't edit content")))
                return new HttpUnauthorizedResult();

            string previousRoute = null;
            if (contentItem.Has<IAliasAspect>()
                && !string.IsNullOrWhiteSpace(returnUrl)
                && Request.IsLocalUrl(returnUrl)
                // only if the original returnUrl is the content itself
                && String.Equals(returnUrl, Url.ItemDisplayUrl(contentItem), StringComparison.OrdinalIgnoreCase)
                ) {
                previousRoute = contentItem.As<IAliasAspect>().Path;
            }

            var model = _contentManager.UpdateEditor(contentItem, this);
            if (!ModelState.IsValid) {
                _transactionManager.Cancel();
                //return View("~/Core/Contents/Views/Admin/Edit.cshtml", model);
                return Fail();
            }

            callback(contentItem);

            if (!string.IsNullOrWhiteSpace(returnUrl)
                && previousRoute != null
                && !String.Equals(contentItem.As<IAliasAspect>().Path, previousRoute, StringComparison.OrdinalIgnoreCase)) {
                returnUrl = Url.ItemDisplayUrl(contentItem);
            }


            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been saved.")
                : T("Your {0} has been saved.", contentItem.TypeDefinition.DisplayName));

            //return this.RedirectLocal(returnUrl, () => RedirectToAction("Edit", new RouteValueDictionary { { "Id", contentItem.Id } }));
            return Done(T("Commited").Text);
        }




        //bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
        //    return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        //}

        //void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
        //    ModelState.AddModelError(key, errorMessage.ToString());
        //}

    }
}