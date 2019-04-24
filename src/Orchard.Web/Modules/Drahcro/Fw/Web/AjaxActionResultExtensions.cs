namespace Rhythm.Web
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using Rhythm.ErrorHandling;

    /// <summary>
    /// 提供对 AjaxActionResult 的扩展方法
    /// </summary>
    public static class AjaxActionResultExtensions
    {
        //public static AjaxActionResult View(this AjaxActionResult ajaxResult, string viewName, object viewModel)
        //{
        //    return ajaxResult;
        //}

        ///// <summary>
        ///// 向 Ajax 结果中添加 View Result，你可以多次调用本方法以添加多个 View Result，然后在客户端通过 ajax response.dataKey 获得 View Result。
        ///// <para>这通常被用于 Ajax 拉取部分 HTML。</para>
        ///// </summary>
        ///// <param name="ajaxResult"></param>
        ///// <param name="key">设置在客户端获取 View Result 的 Key，设置后就可在客户端回掉函数中通过 ajax response.dataKey 获得 View Result。</param>
        ///// <param name="viewResult">要添加的  View Result 信息，通常传 Controller.View() 即可。</param>
        ///// <returns></returns>
        //public static AjaxActionResult AddView(this AjaxActionResult ajaxResult, string key, ViewResultBase viewResult)
        //{
        //    ajaxResult.OnExecutingResult += (controlerContext) =>
        //    {
        //        ajaxResult.Data(key, ExecuteView(controlerContext, viewResult));
        //    };
        //    return ajaxResult;
        //}

        public static string ExecutePartialView(this Controller controller, string viewName)
        {
            return ExecutePartialView(controller, viewName, controller.ViewData.Model);
        }

        public static string ExecutePartialView(this Controller controller, string viewName, object model)
        {
            if (model != controller.ViewData.Model)
            {
                controller.ViewData.Model = model;
            }
            return ExecuteView(controller.ControllerContext, new PartialViewResult
            {
                ViewName = viewName,
                TempData = controller.TempData,
                ViewData = controller.ViewData,
                ViewEngineCollection = controller.ViewEngineCollection,
            });
        }

        public static string ExecuteView(this Controller controller, string viewName)
        {
            return ExecuteView(controller, viewName, controller.ViewData.Model);
        }

        public static string ExecuteView(this Controller controller, string viewName, object model)
        {
            if (model != controller.ViewData.Model)
            {
                controller.ViewData.Model = model;
            }
            return ExecuteView(controller.ControllerContext, new ViewResult
            {
                ViewName = viewName,
                MasterName = null,
                TempData = controller.TempData,
                ViewData = controller.ViewData,
                ViewEngineCollection = controller.ViewEngineCollection,
            });
        }


        public static string ExecuteView(this ControllerContext controllerContext, ViewResultBase viewResult)
        {
            var response = controllerContext.HttpContext.Response;
            var output = response.Output;
            System.IO.StringWriter viewTextWriter = null;
            try
            {
                viewTextWriter = new System.IO.StringWriter();
                response.Output = viewTextWriter;
                viewResult.ExecuteResult(controllerContext);
                return viewTextWriter.ToString();
            }
            finally
            {
                response.Output = output;
                if (viewTextWriter != null)
                {
                    viewTextWriter.Dispose();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ajaxResult"></param>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="errorsMode"></param>
        public static bool ShouldResponseError(this AjaxActionResult ajaxResult, System.Web.HttpContext context, Exception ex, CustomErrorsMode errorsMode)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }
            var exSeverityType = ex.GetSeverity();
            if (exSeverityType == ExceptionSeverity.Error)
            {
                return false;
            }
            bool showCustomError = errorsMode == CustomErrorsMode.On || (errorsMode == CustomErrorsMode.RemoteOnly && !context.Request.IsLocal);
            if (!showCustomError)
            {
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ajaxResult"></param>
        ///// <param name="context"></param>
        ///// <param name="ex"></param>
        ///// <param name="errorsMode"></param>
        ///// 
        //[Obsolete("请使用 ShouldResponseError 和 ResponseError 代替")]
        //public static bool ResponseError(this AjaxActionResult ajaxResult, System.Web.HttpContext context, Exception ex, CustomErrorsMode errorsMode)
        //{
        //    if (ex == null)
        //    {
        //        throw new ArgumentNullException("ex");
        //    }
        //    //var exSeverityType = ex.GetSeverity();
        //    //if (exSeverityType == ExceptionSeverity.Error)
        //    //{
        //    //    return false;
        //    //}
        //    //bool showCustomError = errorsMode == CustomErrorsMode.On || (errorsMode == CustomErrorsMode.RemoteOnly && !context.Request.IsLocal);
        //    //if (!showCustomError)
        //    //{
        //    //    return false;
        //    //}
        //    ResponseError(ajaxResult, context, ex);
        //    return true;
        //}

        public static void ResponseError(this AjaxActionResult ajaxResult, System.Web.HttpContext context, Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }
            //var exceptionMessage = ex.GetCustomErrorMessage() ?? ex.Message;
            //var exceptionMessage = showCustomError ? (ex.GetCustomErrorMessage() ?? ex.Message) : ex.Message;
            SetError(ajaxResult, ex);
            context.ClearError();
            ajaxResult.ExecuteResult(context.Response);
        }

        public static void SetError(AjaxActionResult ajaxResult, Exception ex)
        {
            var modelStateDictionary = ex.GetModelState();
            if (modelStateDictionary != null && modelStateDictionary.Count > 0)
            {
                foreach (var item in modelStateDictionary)
                {
                    ajaxResult.ModelState.Add(item);

                    //var state = item.Value;
                    //var errors = state.Errors;
                    //if (errors == null) continue;

                    //for (int i = 0; i < errors.Count; i++)
                    //{
                    //	//var customErrorMessage = exceptions[i].GetCustomErrorMessage();
                    //	//if (customErrorMessage != null)
                    //	//{
                    //	//    ajaxResult.ModelState.AddModelError(key, customErrorMessage);
                    //	//}
                    //	//else
                    //	//{
                    //	//}
                    //}
                }

            }
            ajaxResult.Failure(ex.Message);

            var exInfo = ex.GetExtraInfo();
            var exSeverityType = exInfo == null ? ExceptionSeverity.None : exInfo.Severity;
            // 如果是一般的验证失败异常
            if (exSeverityType == ExceptionSeverity.Validation)
            {
                ajaxResult.StatusCode((int)System.Net.HttpStatusCode.OK);
            }
            else
            {
                System.Web.HttpException httpEx = ex as System.Web.HttpException;
                // 内部错误异常（500）
                ajaxResult.StatusCode(httpEx == null ? (int)System.Net.HttpStatusCode.InternalServerError : httpEx.GetHttpCode());
            }
            // 设置异常 Id 到 Ajax Response 中，客户端可以获取它，并向用户界面显示。
            var exceptionId = exInfo == null ? null : exInfo.ExceptionId;
            if (exceptionId != null)
            {
                ajaxResult.Item("exceptionId", exceptionId);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ajaxResult"></param>
        /// <param name="context"></param>
        /// <param name="errorMessage"></param>
        public static void ResponseError(this AjaxActionResult ajaxResult, System.Web.HttpContext context, string errorMessage)
        {
            // 内部错误异常（500）
            ajaxResult.StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            ajaxResult.Failure(errorMessage);
            context.ClearError();
            ajaxResult.ExecuteResult(context.Response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorMessage"></param>
        public static void ResponseError(System.Web.HttpContext context, string errorMessage)
        {
            AjaxActionResult ajaxResult = new AjaxActionResult();
            // 内部错误异常（500）
            ajaxResult.StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            ajaxResult.Failure(errorMessage);
            context.ClearError();
            ajaxResult.ExecuteResult(context.Response);
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="ex"></param>
        ///// <param name="errorsMode"></param>
        //public static bool ResponseError(System.Web.HttpContext context, Exception ex, CustomErrorsMode errorsMode)
        //{
        //    if (ex == null)
        //    {
        //        throw new ArgumentNullException("ex");
        //    }
        //    return new AjaxActionResult().ShouldResponseError(context, ex, errorsMode);
        //}
    }

}
