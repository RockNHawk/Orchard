using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization.Services;
using Orchard.Localization;
using Orchard.Localization.Models;
using System;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Layouts.Models;

namespace Pvlt.Localization.Controllers {
    public class AdminController : Controller {
        private readonly IContentManager _contentManager;
        private readonly ILocalizationService _localizationService;

        public AdminController(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            ILocalizationService localizationService,
            IShapeFactory shapeFactory) {
            _contentManager = contentManager;
            _localizationService = localizationService;
            T = NullLocalizer.Instance;
            Services = orchardServices;
            Shape = shapeFactory;
        }

        public dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        /// <summary>
        /// This method shoud be the same than the original 
        /// Orchard.Localization.Controllers.AdminController.Translate
        /// except of the region that copy some content parts
        /// </summary>
        public ActionResult Translate(int id, string to) {
            var masterContentItem = _contentManager.Get(id, VersionOptions.Latest);
            if (masterContentItem == null)
                return HttpNotFound();

            var masterLocalizationPart = masterContentItem.As<LocalizationPart>();
            if (masterLocalizationPart == null)
                return HttpNotFound();

            // Check is current item still exists, and redirect.
            var existingTranslation = _localizationService.GetLocalizedContentItem(masterContentItem, to);
            if (existingTranslation != null) {
                var existingTranslationMetadata = _contentManager.GetItemMetadata(existingTranslation);
                return RedirectToAction(
                    Convert.ToString(existingTranslationMetadata.EditorRouteValues["action"]),
                    existingTranslationMetadata.EditorRouteValues);
            }

            var contentItemTranslation = _contentManager.New<LocalizationPart>(masterContentItem.ContentType);
            contentItemTranslation.MasterContentItem = masterContentItem;

            // Copy some parts from master
            ClonePartsFromMaster(masterContentItem, contentItemTranslation);

            return View(_contentManager.BuildEditor(contentItemTranslation));
        }

        private static void ClonePartsFromMaster(ContentItem masterContentItem, LocalizationPart contentItemTranslation) {
            if (masterContentItem.Has<TitlePart>()) {
                var masterTitlePart = masterContentItem.As<TitlePart>();
                var clonedTitlePart = contentItemTranslation.ContentItem.As<TitlePart>();
                clonedTitlePart.Title = masterTitlePart.Title;
            }

            if (masterContentItem.Has<BodyPart>()) {
                var masterBodyPart = masterContentItem.As<BodyPart>();
                var clonedBodyPart = contentItemTranslation.ContentItem.As<BodyPart>();
                clonedBodyPart.Text = masterBodyPart.Text;
                clonedBodyPart.Format = masterBodyPart.Format;
            }

            if (masterContentItem.Has<LayoutPart>()) {
                var masterLayoutPart = masterContentItem.As<LayoutPart>();
                var clonedLayoutPart = contentItemTranslation.ContentItem.As<LayoutPart>();
                clonedLayoutPart.LayoutData = masterLayoutPart.LayoutData;
                clonedLayoutPart.TemplateId = masterLayoutPart.TemplateId;
            }
        }
    }
}