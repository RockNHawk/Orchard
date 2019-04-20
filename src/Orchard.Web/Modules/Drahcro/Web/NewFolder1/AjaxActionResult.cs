using System;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Rhythm.Web {


    /// <summary>
    /// Ajax返回结果帮助类
    /// </summary>
    public class AjaxActionResult : ActionResult {
        static readonly System.Globalization.CultureInfo invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
        static readonly System.Text.Encoding encodingUTF8 = System.Text.Encoding.UTF8;

        object data;

        Dictionary<string, object> dataDictionary;//= new Dictionary<string, object>();
        //Dictionary<string, ViewResultBase> viewResults;// = new Dictionary<string, ViewResultBase>();
        string customHtml;
        int statusCode;
        string redirect;
        string crossDomain;
        string message;
        int messageShowTime;
        string operationStatus;
        ModelStateDictionary modelState;
        UrlHelper url;
        public Newtonsoft.Json.JsonSerializerSettings ItemsSerializerSettings { get; set; }
        public Newtonsoft.Json.JsonSerializerSettings ModelStateSerializerSettings { get; set; }

        static readonly Newtonsoft.Json.JsonSerializerSettings defaultSerializerSettings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = JsonUtility.Settings.HibernateContractResolver,
        };


        /// <summary>
        /// 
        /// </summary>
        public AjaxActionResult() {
            this.statusCode = 200;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="modelState"></param>
        public AjaxActionResult(ControllerContext actionContext, ModelStateDictionary modelState)
            : this() {
            if (actionContext == null) {
                throw new ArgumentNullException(nameof(actionContext));
            }
            if (modelState == null) {
                throw new ArgumentNullException(nameof(modelState));
            }
            this.modelState = modelState;
            this.url = new UrlHelper(actionContext.RequestContext);
        }




        public AjaxActionResult CrossDomain(string crossDomain) {
            if (crossDomain == null) {
                throw new ArgumentNullException(nameof(crossDomain));
            }
            this.crossDomain = crossDomain;
            return this;
        }

        /// <summary>
        /// 表示当前操作执行成功
        /// </summary>
        /// <param name="msg">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult Succeed(string msg = null, int messageShowTime = 0) {
            //this.succeed = true;
            this.operationStatus = "success";
            this.messageShowTime = messageShowTime;
            if (msg != null) {
                this.Message(msg);
            }
            return this;
        }

        /// <summary>
        /// 表示当前操作执行失败
        /// </summary>
        /// <param name="msg">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult Failure(string msg = null) {
            //this.succeed = false;
            this.operationStatus = "failure";
            if (msg != null) {
                this.Message(msg);
            }
            return this;
        }

        /// <summary>
        /// 设置用于在用户界面上显示的提示消息
        /// </summary>
        /// <param name="msg">用于在用户界面上显示的提示消息</param>
        /// <returns></returns>
        public AjaxActionResult Message(string msg, int messageShowTime = 0) {
            this.message = msg;
            this.messageShowTime = messageShowTime;
            return this;
        }

        /// <summary>
        /// 表示需要跳转。
        /// </summary>
        /// <param name="url">要跳转的 url，“.”表示刷新本页。</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult RedirectUrl(string url) {
            this.redirect = url;
            //dataDictionary["redirect"] = url;
            return this;
        }

        /// <summary>
        /// AJAX 执行完毕后，刷新当前页面
        /// </summary>
        /// <returns></returns>
        public AjaxActionResult Refresh() {
            this.redirect = ".";
            //dataDictionary["redirect"] = ".";
            return this;
        }

        /// <summary>
        /// 表示需要跳转。
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult RedirectAction(string actionName) {
            if (url == null) {
                throw new InvalidOperationException("未能获得 UrlHelper 的实例。");
            }
            this.redirect = url.Action(actionName);
            return this;
        }

        /// <summary>
        /// 表示需要跳转。
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult RedirectAction(string actionName, string controllerName) {
            if (url == null) {
                throw new InvalidOperationException("未能获得 UrlHelper 的实例。");
            }
            this.redirect = url.Action(actionName, controllerName);
            return this;
        }

        ///// <summary>
        ///// 指定要在客户端执行的 HTML 代码，它将被加载到用户页面中
        ///// </summary>
        ///// <returns>AjaxResult</returns>
        //public AjaxActionResult CustomHtml(string customHtml)
        //{
        //    this.customHtml = customHtml;
        //    //this.customHtmlView = null;
        //    return this;
        //}

        ///// <summary>
        ///// 指定要在客户端执行的 HTML 代码，它将被加载到用户页面中
        ///// </summary>
        ///// <returns>AjaxResult</returns>
        //public AjaxActionResult CustomHtml(ViewResultBase viewResult)
        //{
        //    this.customHtml = null;
        //    this.customHtmlView = viewResult;
        //    return this;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public AjaxActionResult StatusCode(int statusCode) {
            this.statusCode = statusCode;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public AjaxActionResult StatusCode(System.Net.HttpStatusCode statusCode) {
            this.statusCode = (int)statusCode;
            return this;
        }

        /// <summary>
        /// 设置需要发送给客户端的自定义数据（Key-Value 键值对），<para></para>
        /// 设置后就可以在客户端通过 ajax response.data.yourKey 获取到值。<para></para>
        /// ajax response 你可以在 &lt;form data-ajax="{ success : function(response,context){ } }" /&gt; 中获取到。
        /// </summary>
        /// <param name="key">数据的键，键值对会被添加到 ajax response.data 中</param>
        /// <param name="value">数据的值，通过 ajax response.data.键 获取</param>
        /// <returns></returns>
        [Obsolete("Use Data Instead")]
        public AjaxActionResult Item(string key, object value) {
            if (this.dataDictionary == null) this.dataDictionary = new Dictionary<string, object>();
            this.dataDictionary[key] = value;
            return this;
        }

        public AjaxActionResult Data(object data) {
            this.data = data;
            return this;
        }

        ///// <summary>
        ///// 向 Ajax 结果中添加 View Result，你可以多次调用本方法以添加多个 View Result，然后在客户端通过 ajax response.dataKey 获得 View Result。
        ///// <para>这通常被用于 Ajax 拉取部分 HTML。</para>
        ///// </summary>
        ///// <param name="ajaxResult"></param>
        ///// <param name="key">设置在客户端获取 View Result 的 Key，设置后就可在客户端回掉函数中通过 ajax response.dataKey 获得 View Result。</param>
        ///// <param name="viewResult">要添加的  View Result 信息，通常传 Controller.View() 即可。</param>
        ///// <returns></returns>
        //public AjaxActionResult AddViewResult(string key, ViewResultBase viewResult)
        //{
        //    if (this.viewResults == null) this.viewResults = new Dictionary<string, ViewResultBase>();
        //    this.viewResults[key] = viewResult;
        //    return this;
        //}


        /// <summary>
        /// 
        /// </summary>
        public System.Collections.Generic.IDictionary<string, object> Items { get { return dataDictionary; } }

        public event Action<ControllerContext> OnExecutingResult;

        public override void ExecuteResult(ControllerContext context) {
            OnExecutingResult?.Invoke(context);
            var response = context.HttpContext.Response;
            if (string.Compare(response.ContentType, "text/html", StringComparison.OrdinalIgnoreCase) != 0) {
                response.ContentType = "text/html";
            }
            response.Write(this.GetString(context));
        }

        //public void ExecuteResult(HttpResponse response)
        //{
        //    if (response == null)
        //    {
        //        throw new ArgumentNullException(nameof(response));
        //    }
        //    if (string.Compare(response.ContentType, "text/html", StringComparison.OrdinalIgnoreCase) != 0)
        //    {
        //        response.ContentType = "text/html";
        //    }
        //    response.WriteAsync(this.GetString(null));
        //}

        public override string ToString() {
            return GetString(null);
        }


        bool isItemEncrypted;

        public AjaxActionResult ItemEncrypted(bool isEncrypt) {
            this.isItemEncrypted = isEncrypt;
            return this;
        }

        string GetString(ControllerContext controllerContext) {
            //if (viewResults != null && viewResults.Count > 0)
            //{
            //    if (controllerContext == null)
            //    {
            //        throw new ArgumentException("当 Ajax Result 中包含 View 时，controllerContext 不应为 null", nameof(controllerContext));
            //    }
            //    foreach (var pair in viewResults)
            //    {
            //        dataDictionary[pair.Key] = controllerContext.ExecuteView(pair.Value);
            //    }
            //}

            string modelStateJsonStr = GetModelStateJsonString(this.modelState);


            var dataObj = dataDictionary != null && dataDictionary.Count > 0 ? dataDictionary : data;

            // base64 加密是因为复杂数据情况下，序列化会出错
            var dataJson = dataObj == null ? null : JsonConvert.SerializeObject(dataObj, ItemsSerializerSettings ?? defaultSerializerSettings);
            var dataJsonResult = dataJson == null ? null : isItemEncrypted ? Convert.ToBase64String(encodingUTF8.GetBytes(dataJson)) : dataJson;
            //string dataJsonStrEncrypted = dataDictionary == null || dataDictionary.Count == 0 ? null : Convert.ToBase64String(encodingUTF8.GetBytes(dataJsonStr));
            //var customHtmlJsonStr = customHtml == null || customHtml.Length == 0 ? null : Convert.ToBase64String(encodingUTF8.GetBytes(customHtml));


            var sb = new System.Text.StringBuilder(100
                + (redirect == null ? 0 : redirect.Length + 5 + 15)
                + (message == null ? 0 : message.Length + 5 + 7)
                + (modelStateJsonStr == null ? 0 : modelStateJsonStr.Length + 5 + 10)
                //+ (customHtmlJsonStr == null ? 0 : customHtmlJsonStr.Length + 5 + 10)
                + (dataJsonResult == null ? 0 : dataJsonResult.Length + 5 + 4))
                ;

            sb.Append('{').Append('\n');
            {
                sb.Append(@"""statusCode"":").Append(statusCode);
                if (operationStatus != null) {
                    sb.Append(@",""operationStatus"":""").Append(this.operationStatus).Append('"');
                }
                if (redirect != null) {
                    sb.Append(@",""redirect"":""").Append(this.redirect).Append('"');
                }
                if (message != null) {
                    if (this.message == null) {
                        sb.Append(@",""message"":null");
                    }
                    else {
                        sb.Append(@",""message"":").Append(Newtonsoft.Json.JsonConvert.SerializeObject(this.message));
                        //sb.Append(@",""message"":""").Append(this.message.Replace("\r", "\\\r").Replace("\n", "\\\n")).Append('"');
                    }
                }
                if (messageShowTime > 0) {
                    sb.Append(@",""messageShowTime"":").Append(this.messageShowTime);
                }
                if (crossDomain != null && crossDomain.Length > 0) {
                    sb.Append(@",""crossDomain"":""").Append(crossDomain).Append('"');
                }
                //if (customHtml != null && customHtml.Length > 0)
                //{
                //    sb.Append('\n').Append(@",""customHtml"":""").Append(customHtmlJsonStr).Append('"');
                //}
                if (modelStateJsonStr != null) {
                    sb.Append('\n').Append(@",""modelState"":""").Append(modelStateJsonStr).Append('"');
                }


                if (isItemEncrypted) {
                    if (dataJsonResult != null) {
                        sb.Append('\n').Append(@",""dataEncrypted"":""").Append(dataJsonResult).Append('"');
                    }
                }
                else {
                    if (dataJsonResult != null) {
                        sb.Append('\n').Append(@",""data"":").Append(dataJsonResult);
                    }
                }

            }
            sb.Append('\n').Append('}');
            return sb.ToString();
        }

        string GetModelStateJsonString(ModelStateDictionary modelState) {
            if (modelState == null || modelState.IsValid) {
                return null;
            }
            var errorOnlyModelStates = new ModelStateDictionary(modelState);
            foreach (var item in modelState) {
                var errors = item.Value.Errors;
                if (errors == null || errors.Count == 0) {
                    continue;
                }
                errorOnlyModelStates.Remove(item.Key);
                //errorOnlyModelStates.AddModelError(item.Key, item.Value);
            }
            if (errorOnlyModelStates.Count == 0) {
                return null;
            }
            return Convert.ToBase64String(encodingUTF8.GetBytes(JsonConvert.SerializeObject(errorOnlyModelStates, ModelStateSerializerSettings ?? defaultSerializerSettings)));
        }

        /// <summary>
        /// 
        /// </summary>
        public ModelStateDictionary ModelState {
            get {
                if (modelState == null) {
                    modelState = new ModelStateDictionary();
                }
                return modelState;
            }
            set { this.modelState = value; }
        }

    }

}
