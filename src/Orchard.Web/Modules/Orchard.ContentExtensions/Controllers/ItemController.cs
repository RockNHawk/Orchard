using Orchard.ContentExtensions.Services;
using Orchard.ContentExtensions.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Contents.Settings;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Html;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Orchard.ContentExtensions.Controllers
{
    [Themed]
    public class ItemController : Controller, IUpdateModel, IViewDataContainer
    {
        private readonly IContentManager contentManager;
        private readonly IWidgetService widgetService;
        private readonly IExtendedContentManager extendedContentManager;
        private readonly ITransactionManager transactionManager;
        private readonly IThemeAwareViewEngine themeAwareViewEngine;
        private readonly IDisplayHelperFactory displayHelperFactory;
        private readonly IPartSerializationManager partSerializationManager;

        public ItemController(
            IOrchardServices services,
            IWidgetService widgetService,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager,
            ITransactionManager transactionManager,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IExtendedContentManager extendedContentManager,
            IThemeAwareViewEngine themeAwareViewEngine,
            IPartSerializationManager partSerializationManager,
            IDisplayHelperFactory displayHelperFactory)
        {
            this.widgetService = widgetService;
            this.partSerializationManager = partSerializationManager;
            this.themeAwareViewEngine = themeAwareViewEngine;
            this.displayHelperFactory = displayHelperFactory;
            this.Services = services;
            this.transactionManager = transactionManager;
            this.extendedContentManager = extendedContentManager;
            this.contentManager = contentManager;
            this.Shape = shapeFactory;
            this.T = NullLocalizer.Instance;
        }

        public dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services;

        public ActionResult Display(int id, string type)
        {
            ContentItem contentItem = null;
            if (string.IsNullOrEmpty(type))
            {
                contentItem = this.contentManager.Get(id, VersionOptions.Published);
            }
            else
            {
                contentItem = this.extendedContentManager.Get(id, type);
            }

            if (contentItem == null)
                return HttpNotFound();

            var viewPermission = Orchard.Core.Contents.Permissions.ViewContent;
            if (!Services.Authorizer.Authorize(viewPermission, contentItem, T("Cannot view content")))
            {
                return new HttpUnauthorizedResult();
            }

             bool isAjaxRequst = this.Request.IsAjaxRequest();

             dynamic model = contentManager.BuildDisplay(contentItem);

            if (isAjaxRequst)
            {
                AjaxMessageViewModel ajaxMessageModel = new AjaxMessageViewModel { IsDone = true };

                this.widgetService.GetWidgets(model, this.HttpContext);

                ajaxMessageModel.Html = this.RenderShapePartial(model, "DisplayAjax");

                return this.Json(ajaxMessageModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return this.View((object)model);
            }
        }

        public ActionResult Create(string id, int? containerId)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var contentItem = this.contentManager.New(id);

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, contentItem, T("Cannot create content")))
                return new HttpUnauthorizedResult();

            if (containerId.HasValue && contentItem.Is<ContainablePart>())
            {
                var common = contentItem.As<CommonPart>();
                if (common != null)
                {
                    common.Container = this.contentManager.Get(containerId.Value);
                }
            }

            bool isAjaxRequest = Request.IsAjaxRequest();

  
            dynamic model = this.contentManager.BuildEditor(contentItem);

            if (isAjaxRequest)
            {
                AjaxMessageViewModel ajaxMessageModel = new AjaxMessageViewModel { IsDone = true };
                this.widgetService.GetWidgets(model, this.HttpContext);
                ajaxMessageModel.Html = ControllerHelper.RenderPartialViewToString(this, "Create", model);

                return this.Json(ajaxMessageModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return this.View((object)model);
            }
        }

        public ActionResult Edit(int id, string type)
        {
            ContentItem contentItem = null;

            if (string.IsNullOrEmpty(type))
            {
                contentItem = this.contentManager.Get(id, VersionOptions.Published);
            }
            else
            {
                contentItem = this.extendedContentManager.Get(id, type);
            }

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, contentItem, T("Cannot edit content")))
                return new HttpUnauthorizedResult();

            bool isAjaxRequest = Request.IsAjaxRequest();

            dynamic model = this.contentManager.BuildEditor(contentItem);

            if (isAjaxRequest)
            {
                AjaxMessageViewModel ajaxMessageModel = new AjaxMessageViewModel { Id = contentItem.Id, IsDone = true };
                this.widgetService.GetWidgets(model, this.HttpContext);
                ajaxMessageModel.Html = ControllerHelper.RenderPartialViewToString(this, "Edit", model);

                return this.Json(ajaxMessageModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return this.View((object)model);
            }
        }

        [HttpPost, ActionName("CreateData")]
        public ActionResult CreatePOST(string id, string returnUrl)
        {
            return CreatePOST(id, returnUrl, contentItem =>
            {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    this.contentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("CreateData")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPOST(string id, string returnUrl)
        {

            // pass a dummy content to the authorization check to check for "own" variations
            var dummyContent = this.contentManager.New(id);

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.PublishContent, dummyContent, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            return CreatePOST(id, returnUrl, contentItem => this.contentManager.Publish(contentItem));
        }

        [HttpPost]
        public ActionResult Remove(int id, string returnUrl)
        {
            var contentItem = this.contentManager.Get(id, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.DeleteContent, contentItem, T("Couldn't remove content")))
                return new HttpUnauthorizedResult();

            if (contentItem != null)
            {
                this.contentManager.Remove(contentItem);
                Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                    ? T("That content has been removed.")
                    : T("That {0} has been removed.", contentItem.TypeDefinition.DisplayName));
            }

            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                return this.Json(this.CreateAjaxMessageModel(contentItem, string.Empty), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.RedirectLocal(returnUrl, () => RedirectToAction("List"));
            }
        }

        private ActionResult CreatePOST(string id, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var contentItem = this.contentManager.New(id);

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, contentItem, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            this.contentManager.Create(contentItem, VersionOptions.Draft);

            dynamic model = this.contentManager.UpdateEditor(contentItem, this);
            if (!ModelState.IsValid)
            {
                this.transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View((object)model);
            }

            conditionallyPublish(contentItem);

            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been created.")
                : T("Your {0} has been created.", contentItem.TypeDefinition.DisplayName));
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return this.RedirectLocal(returnUrl);
            }
            var adminRouteValues = this.contentManager.GetItemMetadata(contentItem).AdminRouteValues;

            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                return this.Json(this.CreateAjaxMessageModel(contentItem, string.Empty), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToRoute(adminRouteValues);
            }
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        [HttpPost, ActionName("EditData")]
        public ActionResult EditPOST(int id, string returnUrl)
        {
            return EditPOST(id, returnUrl, contentItem =>
            {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    this.contentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("EditData")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditAndPublishPOST(int id, string returnUrl)
        {
            var content = this.contentManager.Get(id, VersionOptions.Latest);

            if (content == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.PublishContent, content, T("Couldn't publish content")))
                return new HttpUnauthorizedResult();

            return EditPOST(id, returnUrl, contentItem => this.contentManager.Publish(contentItem));
        }

        private ActionResult EditPOST(int id, string returnUrl, Action<ContentItem> conditionallyPublish)
        {
            var contentItem = this.contentManager.Get(id, VersionOptions.DraftRequired);

            if (contentItem == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.EditContent, contentItem, T("Couldn't edit content")))
                return new HttpUnauthorizedResult();

            string previousRoute = null;
            if (contentItem.Has<IAliasAspect>()
                && !string.IsNullOrWhiteSpace(returnUrl)
                && Request.IsLocalUrl(returnUrl)
                // only if the original returnUrl is the content itself
                && String.Equals(returnUrl, Url.ItemDisplayUrl(contentItem), StringComparison.OrdinalIgnoreCase)
                )
            {
                previousRoute = contentItem.As<IAliasAspect>().Path;
            }

            dynamic model = this.contentManager.UpdateEditor(contentItem, this);
            if (!ModelState.IsValid)
            {
                this.transactionManager.Cancel();
                // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
                return View("Edit", (object)model);
            }

            conditionallyPublish(contentItem);

            if (!string.IsNullOrWhiteSpace(returnUrl)
                && previousRoute != null
                && !String.Equals(contentItem.As<IAliasAspect>().Path, previousRoute, StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = Url.ItemDisplayUrl(contentItem);
            }

            Services.Notifier.Information(string.IsNullOrWhiteSpace(contentItem.TypeDefinition.DisplayName)
                ? T("Your content has been saved.")
                : T("Your {0} has been saved.", contentItem.TypeDefinition.DisplayName));

            bool isAjaxRequest = Request.IsAjaxRequest();

            if (isAjaxRequest)
            {
                return this.Json(this.CreateAjaxMessageModel(contentItem, string.Empty), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.RedirectLocal(returnUrl, () => RedirectToAction("Edit", new RouteValueDictionary { { "Id", contentItem.Id } }));
            }
        }

        private AjaxMessageViewModel CreateAjaxMessageModel(ContentItem contentItem, string returnedShape)
        {
            AjaxMessageViewModel ajaxMessageModel = new AjaxMessageViewModel { Id = contentItem.Id, IsDone = true, Data = this.partSerializationManager.Convert(contentItem) };

            if (!string.IsNullOrEmpty(returnedShape))
            {
                dynamic returnedShapeModel = new Composite();
                returnedShapeModel.ContentItem = contentItem;
                returnedShapeModel.IsAlternative = false;
                ajaxMessageModel.Html = this.RenderShapePartial(returnedShapeModel, returnedShape);
            }

            return ajaxMessageModel;
        }

        private string RenderShapePartial(dynamic model, string shapeType)
        {
            var shape = this.Shape.Partial(TemplateName: shapeType, Model: model);
            var display = this.GetDisplayHelper();
            return Convert.ToString(display(shape));
        }

        private dynamic GetDisplayHelper()
        {
            // We can specify any view name, just to get a View only, the shape template finding will be taken care by DisplayHelperFactory.
            // Here the "Brandking" view is always existed, we can also use something like "Layout" ...
            var viewResult = themeAwareViewEngine.FindPartialView(this.ControllerContext, "Layout", false, false);
            var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, new StringWriter());
            return displayHelperFactory.CreateHelper(viewContext, new ViewDataContainer { ViewData = viewContext.ViewData });
        }

        private class ViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}