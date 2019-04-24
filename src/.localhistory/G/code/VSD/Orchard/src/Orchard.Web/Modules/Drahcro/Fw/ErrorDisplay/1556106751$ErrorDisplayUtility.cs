//using System;
//using System.Web;
//using System.Web.Hosting;
//using System.Diagnostics;
//using System.Security.Permissions;
//using System.IO;
//using Rhythm.ErrorHandling;
//using Rhythm;

//namespace Drahcro.ErrorHandling
//{

//    public class ErrorDisplayUtility
//    {

//        public static bool ShouldShowErrorDetail(ErrorDisplayContext md)
//        {
//            var cfg = md.Configuration;
//            return ShouldShowErrorDetail(cfg, md.WorkContext);
//        }

//        public static bool ShouldShowErrorDetail(ErrorDisplayConfiguration cfg, IContext wd)
//        {
//            var mode = cfg == null ? ErrorDisplayMode.ShowCustomErrorRemoteOnly : cfg.DisplayMode;
//            return ShouldShowErrorDetail(mode, wd);
//        }

//        public static bool ShouldShowErrorDetail(ErrorDisplayMode mode, IContext wc)
//        {
//            switch (mode)
//            {
//                case ErrorDisplayMode.ShowCustomErrorRemoteOnly:
//                    return IsLocalhost(wc);
//                case ErrorDisplayMode.ShowCustomError:
//                    return false;
//                case ErrorDisplayMode.ShowErrorDetail:
//                    return true;
//                default:
//                    return true;
//            }
//        }

//        public static bool IsLocalhost(ErrorDisplayContext md)
//        {
//            var wc = md.WorkContext;
//            return md.IsLocal || IsLocalhost(wc);
//        }

//        public static bool IsLocalhost(IContext wc)
//        {
//            if (wc != null)
//            {
//                HttpContext http;
//                try
//                {
//                    http = wc.HttpContext;
//                }
//                catch
//                {
//                    return false;
//                }
//                if (http != null)
//                {
//                    return IsLocalhost(http);
//                }
//            }
//            return false;
//        }

//        public static bool IsLocalhost(HttpContext http)
//        {
//            try
//            {
//                return http.Request.Url.IsLoopback;
//            }
//            catch
//            {
//                return false;
//            }
//        }


//        public static void WriteResponse(IErrorDisplayResult display, HttpResponseBase response)
//        {
//            if (display == null) throw new ArgumentNullException(nameof(display));
//            if (response == null) throw new ArgumentNullException(nameof(response));
//            response.Write(display.ToString());
//            SetResponse(display, response);
//        }

//        public static void SetResponse(IErrorDisplayResult display, HttpResponseBase response)
//        {
//            if (display == null) throw new ArgumentNullException(nameof(display));
//            if (response == null) throw new ArgumentNullException(nameof(response));
//            if (display.HttpStatusCode != 0)
//            {
//                response.StatusCode = display.HttpStatusCode;
//            }
//            if (display.ContentType != null)
//            {
//                response.ContentType = display.ContentType;
//            }
//        }

//        public static string MakeHttpLinePragma(string virtualPath)
//        {
//            string str = "http://server";
//            if (virtualPath != null && !virtualPath.StartsWith("/", StringComparison.Ordinal))
//            {
//                str += "/";
//            }
//            return new Uri(str + virtualPath).ToString();
//        }

//        public string GetPreferredRenderingType(HttpContext context)
//        {
//            HttpRequest httpRequest = (context != null) ? context.Request : null;
//            HttpBrowserCapabilities httpBrowserCapabilities = null;
//            try
//            {
//                httpBrowserCapabilities = ((httpRequest != null) ? httpRequest.Browser : null);
//            }
//            catch
//            {
//                string empty = string.Empty;
//                return empty;
//            }
//            if (httpBrowserCapabilities == null)
//            {
//                return string.Empty;
//            }
//            return httpBrowserCapabilities["preferredRenderingType"];
//        }


//        public static bool HasFilePermission(string path, bool writePermissions)
//        {
//            //if (HttpRuntime.TrustLevel == null && HttpRuntime.InitializationException != null)
//            //{
//            //    return true;
//            //}
//            var ns = NamedPermissionSet;
//            if (ns == null)
//            {
//                return true;
//            }
//            bool result = false;
//            var permission = ns.GetPermission(typeof(FileIOPermission));
//            if (permission != null)
//            {
//                System.Security.IPermission permission2 = null;
//                try
//                {
//                    if (!writePermissions)
//                    {
//                        permission2 = new FileIOPermission(FileIOPermissionAccess.Read, path);
//                    }
//                    else
//                    {
//                        permission2 = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
//                    }
//                }
//                catch
//                {
//                    return false;
//                }
//                result = permission2.IsSubsetOf(permission);
//                return result;
//            }
//            return result;
//        }

//        public static bool FileExists(string filename)
//        {
//            bool result = false;
//            try
//            {
//                result = File.Exists(filename);
//            }
//            catch
//            {
//            }
//            return result;
//        }

//        public static string GetVirtualPathFromHttpLinePragma(string linePragma)
//        {
//            if (string.IsNullOrEmpty(linePragma))
//            {
//                return null;
//            }
//            try
//            {
//                Uri uri = new Uri(linePragma);
//                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
//                {
//                    return uri.LocalPath;
//                }
//            }
//            catch
//            {
//            }
//            return null;
//        }
//        public static string ResolveHttpFileName(string linePragma)
//        {
//            string virtualPathFromHttpLinePragma = GetVirtualPathFromHttpLinePragma(linePragma);
//            if (virtualPathFromHttpLinePragma == null)
//            {
//                return linePragma;
//            }
//            //return HostingEnvironment.MapPathInternal(virtualPathFromHttpLinePragma);
//            return HostingEnvironment.MapPath(virtualPathFromHttpLinePragma);
//        }
//        public static readonly System.Security.NamedPermissionSet NamedPermissionSet = HttpRuntime.GetNamedPermissionSet();
//        public static bool HasAspNetHostingPermission(AspNetHostingPermissionLevel level)
//        {
//            if (NamedPermissionSet == null)
//            {
//                return true;
//            }
//            AspNetHostingPermission aspNetHostingPermission = (AspNetHostingPermission)NamedPermissionSet.GetPermission(typeof(AspNetHostingPermission));
//            return aspNetHostingPermission != null && aspNetHostingPermission.Level >= level;
//        }

//        public static string GetFileName(StackFrame sf)
//        {
//            try
//            {
//                return sf.GetFileName();
//            }
//            catch (System.Security.SecurityException)
//            {
//                return null;
//            }
//        }
//    }

//}