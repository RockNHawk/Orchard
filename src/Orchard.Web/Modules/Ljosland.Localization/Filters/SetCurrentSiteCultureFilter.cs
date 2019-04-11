using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Mvc.Filters;
using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;
using System.Linq;
using Orchard.Localization.Services;
using Ljosland.Localization.Services;
using System;

namespace Ljosland.Localization.Filters
{
    [OrchardFeature("Localized.Blogs")]
    public class SetCurrentSiteCultureFilter : FilterProvider, IResultFilter
    {
        private readonly IContentManager _contentManager;

        public SetCurrentSiteCultureFilter(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        //Saves culture information to cookie
        private void SaveCultureCookieAndRedirect(string cultureName, HttpContextBase httpContext)
        {
            var cultureCookie = new HttpCookie(CookieCultureSelector.CultureCookieName);
            cultureCookie.Values.Add(CookieCultureSelector.CurrentCultureFieldName, cultureName);
            cultureCookie.Expires = DateTime.Now.AddYears(1);
            httpContext.Response.Cookies.Add(cultureCookie);

            httpContext.Response.Redirect(httpContext.Request.Url.PathAndQuery);
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var cultureCookie = filterContext.RequestContext.HttpContext.Request.Cookies[filterContext.RequestContext.HttpContext.Request.AnonymousID + CookieCultureSelector.CultureCookieName];

            if (cultureCookie != null)
                return; //Current culture is allready set

            var cultureName = string.Empty;
            
            if(TryGetCultureNameFromBlog(filterContext, out cultureName))
                SaveCultureCookieAndRedirect(cultureName, filterContext.RequestContext.HttpContext);
            if(TryGetCultureNameFromAnyItem(filterContext, out cultureName))
                SaveCultureCookieAndRedirect(cultureName, filterContext.RequestContext.HttpContext);

        }

        private bool TryGetCultureNameFromBlog(ResultExecutingContext filterContext, out string cultureName)
        {
            cultureName = string.Empty;
            if (filterContext.RouteData.Values.ContainsKey("blogId"))
            {
                var blogId = int.Parse(filterContext.RouteData.Values["blogId"].ToString());

                var localizedBlog = _contentManager.Query("Blog").Join<LocalizationPartRecord>()
                    .Where(x => x.Id == blogId).List<LocalizationPart>().SingleOrDefault();
                if (localizedBlog != null)
                {
                    cultureName = localizedBlog.Culture.Culture;
                    return true;
                }
            }
            return false;
        }

        private bool TryGetCultureNameFromAnyItem(ResultExecutingContext filterContext, out string cultureName)
        {
            cultureName = string.Empty;
            if (filterContext.RouteData.Values.ContainsKey("Id"))
            {
                var contentItemId = int.Parse(filterContext.RouteData.Values["Id"].ToString());

                var localizedPart = _contentManager.Query().Join<LocalizationPartRecord>()
                    .Where(x => x.Id == contentItemId).List<LocalizationPart>().SingleOrDefault();
                if (localizedPart != null)
                {
                    cultureName = localizedPart.Culture.Culture;
                    return true;
                }
            }
            return false;
        }
    }
}