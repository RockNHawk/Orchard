using System;
using System.Web;
using Orchard.Localization.Services;
using Orchard.Environment.Extensions;

namespace Ljosland.Localization.Services
{
    [OrchardFeature("Localized.Blogs")]
    public class CookieCultureSelector : ICultureSelector
    {
        public const string CultureCookieName = "cultureData";
        public const string CurrentCultureFieldName = "currentCulture";
        public const int SelectorPriority = -2; //priority is higher than SiteCultureSelector priority (-5), but lower then culture picker (-3)

        #region ICultureSelector Members

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            if (context == null || context.Request == null || context.Request.Cookies == null)
            {
                return null;
            }

            HttpCookie cultureCookie = context.Request.Cookies[context.Request.AnonymousID + CultureCookieName];

            if (cultureCookie == null)
            {
                return null;
            }

            string currentCultureName = cultureCookie[CurrentCultureFieldName];
            return String.IsNullOrEmpty(currentCultureName) ? null : new CultureSelectorResult { Priority = SelectorPriority, CultureName = currentCultureName };
        }

        #endregion
    }
}