using System;
using System.Web;
using Rhythm.Web;

namespace Drahcro.Web {
    public class HttpApplicationUtility {
        static readonly string webRoot = HttpApplicationEx.WebRoot;


        public static Exception GetException(System.Web.HttpApplication app, HttpContextBase context) {
            Exception ex;
            if (context == null) {
                //customErrorsMode = GetCustomErrorsMode();
                ex = GetHttpApplicationLastError(app);
            }
            else {
                ex = context.Server.GetLastError();
                //customErrorsMode = context.IsCustomErrorEnabled ? System.Web.Configuration.CustomErrorsMode.On : System.Web.Configuration.CustomErrorsMode.Off;
            }
            if (ex == null) {
                return null;
            }
            else if (ex is System.Reflection.TargetInvocationException && ex.InnerException != null) {
                return ex.InnerException;
            }
            else {
                return ex;
            }
        }

        static bool httpApplicationLastErrorInited;
        static System.Reflection.PropertyInfo httpApplicationLastError;

        public static Exception GetHttpApplicationLastError(System.Web.HttpApplication app) {
            if (app == null) {
                return null;
            }
            if (app.Application != null) {
                var le = app.Application["__LastError"];
                if (le != null) {
                    return (Exception)le;
                }
            }
            if (!httpApplicationLastErrorInited) {
                httpApplicationLastError = typeof(System.Web.HttpApplication).GetProperty("LastError", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                httpApplicationLastErrorInited = true;
            }
            if (httpApplicationLastError == null) {
                return null;
            }
            try {
                return (Exception)httpApplicationLastError.GetValue(app, null);
            }
            catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// 验证是否是静态文件
        /// </summary>
        /// <param name="request">请求 URL</param>
        /// <returns>如果请求的 URL 是静态，则为 true</returns>
        public static bool IsStaticUrl(HttpRequest request) {
            if (!request.Url.IsFile) {
                return false;
            }
            var path = request.Path;
            if (path == null) {
                return false;
            }
            if (path.Length == 1) {
                return false;
            }
            if (webRoot == null || webRoot.Length == 0 || (webRoot.Length == 1 && webRoot[0] == '/')) {
                return
                     path.StartsWith("/Content", StringComparison.Ordinal)
                || path.StartsWith("/Styles", StringComparison.Ordinal)
                || path.StartsWith("/Scripts", StringComparison.Ordinal)
                || path.StartsWith("/bundles", StringComparison.Ordinal)
                || path.StartsWith(".css.aspx", StringComparison.Ordinal)
                || path.StartsWith(".js.aspx", StringComparison.Ordinal)
                || path == ("/favicon.ico")
                || path.EndsWith(".css", StringComparison.Ordinal);
            }
            else {
                return path.StartsWith(webRoot + "Content", StringComparison.Ordinal)
                || path.StartsWith(webRoot + "Styles", StringComparison.Ordinal)
                || path.StartsWith(webRoot + "Scripts", StringComparison.Ordinal)
                || path.StartsWith(webRoot + "bundles", StringComparison.Ordinal)
                || path.StartsWith(".css.aspx", StringComparison.Ordinal)
                || path.StartsWith(".js.aspx", StringComparison.Ordinal)
                || path == ("favicon.ico");
            }
        }

    }
}