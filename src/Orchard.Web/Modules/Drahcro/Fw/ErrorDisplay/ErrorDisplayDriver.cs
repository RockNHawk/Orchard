using System;
using System.Collections.Generic;
using Rhythm.Text;
using Rhythm.Text.Templating;
using System.Web;
using Rhythm.IO;
using Rhythm.Web;

namespace Rhythm.ErrorHandling
{
    public class ErrorDisplayDriver : IErrorDisplayDriver, IDisposable
    {
        struct Entry
        {
            public Lazy<FileBasedVolatileString> Tpl;
            public Lazy<ITemplateRender<ErrorDisplayContext>> Render;
        }

        public const string ExceptionStackTplFilePath =
            @"~/Modules/Rhythm.Foundation.Web/Web/ErrorDisplay/ExceptionStack.cshtml";
        public const string AjaxModelStateTplTplFilePath =
            @"~/Modules/Rhythm.Foundation.Web/Web/ErrorDisplay/Ajax_ModelState.cshtml";
        public const string CustomErrorDefaultTplFilePath =
         @"~/Modules/Rhythm.Foundation.Web/Web/ErrorDisplay/CustomError.Default.cshtml";
        public const string CustomError500TplFilePath =
            @"~/Modules/Rhythm.Foundation.Web/Web/ErrorDisplay/CustomError.Http.500.cshtml";
        public const string CustomError404TplFilePath =
            @"~/Modules/Rhythm.Foundation.Web/Web/ErrorDisplay/CustomError.Http.404.cshtml";

        public static ErrorDisplayDriver Instance { get; set; }

        Entry exceptionStack;
        Entry ajaxModelState;
        Entry customErrorDefault;
        Dictionary<string, Entry> customErrors;

        public ErrorDisplayDriver() : this(
            new Lazy<FileBasedVolatileString>(() => FileBasedVolatileString.FromFile(PathUtility.MapToAbsolutePath(ErrorDisplayDriver.ExceptionStackTplFilePath)), true),
            new Lazy<FileBasedVolatileString>(() => FileBasedVolatileString.FromFile(PathUtility.MapToAbsolutePath(ErrorDisplayDriver.AjaxModelStateTplTplFilePath)), true),
            new Lazy<FileBasedVolatileString>(() => FileBasedVolatileString.FromFile(PathUtility.MapToAbsolutePath(ErrorDisplayDriver.CustomErrorDefaultTplFilePath)), true),
            new Dictionary<string, Lazy<FileBasedVolatileString>>(2, StringComparer.Ordinal)
            {
                ["Http:500"] = new Lazy<FileBasedVolatileString>(() => FileBasedVolatileString.FromFile(PathUtility.MapToAbsolutePath(ErrorDisplayDriver.CustomError500TplFilePath)), true),
                ["Http:404"] = new Lazy<FileBasedVolatileString>(() => FileBasedVolatileString.FromFile(PathUtility.MapToAbsolutePath(ErrorDisplayDriver.CustomError404TplFilePath)), true),
            }
            )
        {

        }

        /// <summary>
        /// TODO: portl/management/etc displayRenderProvider
        /// </summary>
        /// <param name="exceptionStackTpl"></param>
        /// <param name="ajaxModelStateTpl"></param>
        /// <param name="customErrorTpl"></param>
        public ErrorDisplayDriver(
            Lazy<FileBasedVolatileString> exceptionStackTpl,
            Lazy<FileBasedVolatileString> ajaxModelStateTpl,
            Lazy<FileBasedVolatileString> customErrorDefaultTpl,
            Dictionary<string, Lazy<FileBasedVolatileString>> customErrorTpls
            )
        {
            if (exceptionStackTpl == null) throw new ArgumentNullException(nameof(exceptionStackTpl));
            if (ajaxModelStateTpl == null) throw new ArgumentNullException(nameof(ajaxModelStateTpl));
            if (customErrorDefaultTpl == null) throw new ArgumentNullException(nameof(customErrorDefaultTpl));


            this.exceptionStack = Create(exceptionStackTpl);
            this.ajaxModelState = Create(ajaxModelStateTpl);
            this.customErrorDefault = Create(customErrorDefaultTpl);
            if (customErrorTpls != null)
            {
                this.customErrors = new Dictionary<string, Entry>(customErrorTpls.Count);
                //customErrors.ToDictionary(x => x.Key, x => Create(x.Value));
                foreach (var item in customErrorTpls)
                {
                    this.customErrors.Add(item.Key, Create(item.Value));
                }
            }
        }


        static Entry Create(Lazy<FileBasedVolatileString> tpl)
        {
            return new Entry { Tpl = tpl, Render = Compile(tpl) };
        }

        static Lazy<ITemplateRender<ErrorDisplayContext>> Compile(Lazy<FileBasedVolatileString> tpl)
        {
            return new Lazy<ITemplateRender<ErrorDisplayContext>>(() => TemplateUtility2.Compile<ErrorDisplayContext>(new TemplateDef { Code = tpl.Value, }), true);
        }

        public IErrorDisplayResult Display(ErrorDisplayContext context)
        {
            var exception = GetException(context);
            if (exception == null)
            {
                return new HttpErrorDisplayResult("An error occurred but no message to display")
                {
                    ContentType = "text/html",
                    HttpStatusCode = 500
                };
            }
            else
            {
                bool isAjaxRequest = IsAjax(context);
                //if (isAjaxRequest && exception is ModelStateException)
                if (isAjaxRequest && exception.GetSeverity() == ExceptionSeverity.Validation)
                {
                    return new HttpErrorDisplayResult(RenderAjaxModelState(context)) { ContentType = "application/json", HttpStatusCode = 200 };
                }
                else
                {
                    return RenderExceptionStack(context);
                }
            }
        }

        private static Exception GetException(ErrorDisplayContext context)
        {
            var ex = context.Exception;
            if (ex == null) return null;
            return context.Exception = GetException(ex);
        }

        public static Exception GetException(Exception ex)
        {
            var ae = ex as AggregateException;
            if (ae != null)
            {
                if (ae.InnerExceptions != null && ae.InnerExceptions.Count == 1)
                {
                    var innerEx = ae.InnerExceptions[0];
                    if (innerEx is AggregateException && innerEx != ex)
                    {
                        return GetException(innerEx);
                    }
                    else
                    {
                        return innerEx;
                    }
                }
                else
                {
                    return ae;
                }
            }
            else
            {
                return ex;
            }
        }

        internal HttpErrorDisplayResult RenderExceptionStack(ErrorDisplayContext md)
        {
            bool show = ErrorDisplayUtility.ShouldShowErrorDetail(md);
            var ex = md.Exception;
            if (show)
            {
                if (Runtime.IsMono)
                {
                    return null;// new HttpErrorDisplayResult(ex.ToString()) { ContentType = "text/html", HttpStatusCode = 500 };
                }
                else
                {
                    return Render(exceptionStack.Render.Value, md, 500);
                }
            }
            else
            {
                var ce = this.customErrors;
                if (ce != null && ce.Count > 0)
                {
                    var httpEx = ex as HttpException;
                    if (httpEx != null)
                    {
                        var statuscode = httpEx.GetHttpCode();
                        var key = "Http:" + statuscode;
                        Entry handler;
                        if (!ce.TryGetValue(key, out handler))
                        {
                            handler = customErrorDefault;
                        }
                        return Render(handler.Render.Value, md, statuscode);
                    }
                }
                return Render(this.customErrorDefault.Render.Value, md, 500);
            }
        }

        static HttpErrorDisplayResult Render(ITemplateRender<ErrorDisplayContext> render, ErrorDisplayContext md, int statuscode)
        {
            return new HttpErrorDisplayResult(render.Render(md))
            {
                ContentType = "text/html",
                HttpStatusCode = statuscode
            };
        }

        internal string RenderAjaxModelState(ErrorDisplayContext md)
        {
            var result = new AjaxActionResult();
            AjaxActionResultExtensions.SetError(result, md.Exception);
            return result.ToString();
            //return ajaxModelState.Render.Value.Render(md);
        }


        public static bool IsAjax(ErrorDisplayContext context)
        {
            var wc = context.WorkContext;
            if (wc == null) return false;
            var httpContext = wc.HttpContext;
            if (httpContext == null) return false;
            return httpContext.Request.IsAjaxRequest();
        }


        ~ErrorDisplayDriver()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed;

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Dispose(this.exceptionStack);
                this.exceptionStack = default(Entry);

                Dispose(this.ajaxModelState);
                this.exceptionStack = default(Entry);

                Dispose(this.customErrorDefault);
                this.customErrorDefault = default(Entry);

                var ce = this.customErrors;
                if (ce != null)
                {
                    foreach (var item in ce)
                    {
                        Dispose(item.Value);
                    }
                    this.customErrors = null;
                }
                disposed = true;
            }
        }

        private static void Dispose(Entry entry)
        {
            if (entry.Tpl.IsValueCreated)
            {
                entry.Tpl.Value.Dispose();
            }
            entry.Tpl = null;
            if (entry.Render.IsValueCreated)
            {
                var obj = entry.Render as IDisposable;
                if (obj != null)
                {
                    obj.Dispose();
                }
            }
            entry.Render = null;
        }
    }
}
