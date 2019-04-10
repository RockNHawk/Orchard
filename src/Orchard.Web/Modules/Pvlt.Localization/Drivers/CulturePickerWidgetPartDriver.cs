using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Alias;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Configuration;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Mvc.Html;
using Pvlt.Localization.Models;

namespace Pvlt.Localization.Drivers {
    public class CulturePickerWidgetPartDriver : ContentPartDriver<CulturePickerWidgetPart> {
        private readonly IOrchardServices _services;
        private readonly IAliasService _aliasService;
        private readonly ILocalizationService _localizationService;
        private readonly ICultureManager _cultureManager;
        private readonly ShellSettings _tennatSettings;
        private readonly IHomeAliasService _homeAliasService;

        public CulturePickerWidgetPartDriver(
            IOrchardServices services,
            IAliasService aliasService,
            ILocalizationService localizationService,
            ICultureManager cultureManager,
            ShellSettings tennatSettings,
            IHomeAliasService homeAliasService) {
            _services = services;
            _aliasService = aliasService;
            _localizationService = localizationService;
            _cultureManager = cultureManager;
            _tennatSettings = tennatSettings;
            _homeAliasService = homeAliasService;
        }

        protected override DriverResult Display(CulturePickerWidgetPart part, string displayType, dynamic shapeHelper) {
            var currentTenantUrlPréfix = _tennatSettings.RequestUrlPrefix;
            var supportedCultures = _cultureManager.ListCultures();
            var supportedCulturesInfos = supportedCultures.Select(x => new CultureInfo(x)).ToArray();
            var currentCulture = _services.WorkContext.CurrentCulture;
            var currentAlias = _services.WorkContext
                .HttpContext.Request
                .AppRelativeCurrentExecutionFilePath
                .Replace("~/", "");

            if (!string.IsNullOrEmpty(currentTenantUrlPréfix) && currentAlias.StartsWith(currentTenantUrlPréfix)) {
                currentAlias = currentAlias.Substring(
                    startIndex: currentTenantUrlPréfix.Length,
                    length: currentAlias.Length - currentTenantUrlPréfix.Length);

                if (currentAlias.StartsWith("/")) {
                    currentAlias = currentAlias.Substring(1, currentAlias.Length - 1);
                }
            }

            // retrieve current content item route from alias
            var currentAliasRoute = _aliasService.Get(currentAlias);
            if (currentAliasRoute == null) { // no route => no translations !
                return ContentShape("Parts_CulturePicker", () => shapeHelper.Parts_CulturePicker(
                    ViewModel: null,
                    Log: string.Format("Could not retrieve RouteValueDictonary from current alias ({0})", currentAlias)
                ));
            }

            // get route patameters to know wether it's a content item
            var area = (string)currentAliasRoute["area"];
            var controller = (string)currentAliasRoute["controller"];
            var action = (string)currentAliasRoute["action"];
            var id = (string)currentAliasRoute["id"];
            int numericId;
            var isNumericId = int.TryParse(id, out numericId);

            // not a content item => no translations !
            if (area != "Contents" || controller != "Item" || action != "Display" || !isNumericId) {
                return ContentShape("Parts_CulturePicker", () => shapeHelper.Parts_CulturePicker(
                    ViewModel: null,
                    Log: string.Format("The RouteValueDictonary does not specifies a content item (area: {0}, controller: {1}, action: {2}, id: {3})", area, controller, action, id)
                ));
            }

            var contentItem = _services.ContentManager.Get(numericId);
            if (contentItem == null) { // no content items => no translations !
                return ContentShape("Parts_CulturePicker", () => shapeHelper.Parts_CulturePicker(
                    ViewModel: null,
                    Log: string.Format("Could not retrieve Content item with id {0}", id)
                ));
            }

            var currentLocalizationPart = contentItem.As<LocalizationPart>();

            var urlHelper = new UrlHelper(_services.WorkContext.HttpContext.Request.RequestContext);
            var localizations = _localizationService.GetLocalizations(contentItem);

            if (currentLocalizationPart != null && currentLocalizationPart.Culture != null) {
                localizations = localizations.Concat(new[] { currentLocalizationPart });
            }

            var localizationParts = localizations as LocalizationPart[] ?? localizations.ToArray();

            /*
             * build view model 
             */
            var currentOrDefaultCulture = currentLocalizationPart != null
                ? (currentLocalizationPart.Culture != null
                    ? currentLocalizationPart.Culture.Culture
                    : currentCulture)
                : currentCulture;
            var viewModel = new CulturePickerWidgetViewModel {
                CurrentCulture = new CultureInfo(currentOrDefaultCulture),
                Links = supportedCulturesInfos.Select(x => new CulturePickerLink {
                    Culture = x
                }).ToList()
            };

            // foreach implemented culture, try to retrieve
            // related content translation from current content item
            // if not exists => redirect to home page
            foreach (var link in viewModel.Links) {
                var linkCultureName = link.Culture.Name;
                var relatedTranslation = localizationParts.FirstOrDefault(x =>
                    (x.Culture != null ? x.Culture.Culture : "") == linkCultureName
                );
                if (relatedTranslation == null) {
                    link.Url = GetHomePageUrl(urlHelper, linkCultureName);
                    continue;
                }
                link.Url = urlHelper.ItemDisplayUrl(relatedTranslation.ContentItem);
            }

            return ContentShape("Parts_CulturePicker", () => shapeHelper.Parts_CulturePicker(
                    ViewModel: viewModel
            ));
        }

        private string GetHomePageUrl(UrlHelper urlHelper, string culturenName) {
            var homePage = _homeAliasService.GetHomePage();
            if (homePage == null) {
                return "#";
            }

            if (!homePage.Has<ILocalizableAspect>()) {
                return urlHelper.ItemDisplayUrl(homePage);
            }

            var localizableAspects = _localizationService.GetLocalizations(homePage)
                .Concat(new[] { homePage.As<ILocalizableAspect>() })
                .ToArray();

            var currentLocalizableAspect = localizableAspects
                .FirstOrDefault(x => x.Culture == culturenName);

            return urlHelper.ItemDisplayUrl(currentLocalizableAspect != null
                ? currentLocalizableAspect.ContentItem
                : homePage);
        }

        protected override DriverResult Editor(CulturePickerWidgetPart part, dynamic shapeHelper) {
            return ContentShape("Parts_CulturePicker_Edit", () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/CulturePicker",
                Model: part,
                Prefix: Prefix
            ));
        }

        protected override DriverResult Editor(CulturePickerWidgetPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
