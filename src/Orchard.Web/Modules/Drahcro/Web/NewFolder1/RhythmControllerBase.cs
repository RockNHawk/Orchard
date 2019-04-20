using Rhythm.Reflection;
using System;
//using Rhythm.Security.AccessControl;
using Rhythm.Globalization;
//using Rhythm.Data.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web.Mvc;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//public class AdvancedCriteriaDef {

//}
namespace Rhythm.Web
{
    /// <summary>
    /// MVC Controller基类
    /// 添加Controller请继承此类
    /// </summary>
    public class RhythmControllerBase : Controller
    {
        private int firstActionExecute;

        //protected override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    if (firstActionExecute == 0)
        //    {
        //        OnInit(this.WorkContext);
        //        firstActionExecute++;
        //    }

        //    base.OnActionExecuting(context);
        //}

        ///// <summary>Called before the action method is invoked.</summary>
        ///// <param name="context">The action executing context.</param>
        ///// <param name="next">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate" /> to execute. Invoke this delegate in the body
        ///// of <see cref="M:Microsoft.AspNetCore.Mvc.Controller.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)" /> to continue execution of the action.</param>
        ///// <returns>A <see cref="T:System.Threading.Tasks.Task" /> instance.</returns>
        //[NonAction]
        //public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        //{
        //    if (firstActionExecute == 0)
        //    {
        //        OnInit(this.WorkContext);
        //        firstActionExecute++;
        //    }

        //    await base.OnActionExecutionAsync(context, next);
        //}

        protected virtual void OnInit(IContext context)
        {
        }

        ///// <summary>
        ///// TODO:  RepositoryContextManager should initialie in begin request
        ///// </summary>
        //RepositoryContextManager _repositoryContext;

        //protected RepositoryContextManager RepositoryContext
        //{
        //    get
        //    {
        //        if (_repositoryContext == null)
        //        {
        //            _repositoryContext = (RepositoryManager.Default.BeginContext(this.WorkContext));
        //        }

        //        return _repositoryContext;
        //    }
        //}


        //QueryManager query;

        //protected QueryManager Query
        //{
        //    get
        //    {
        //        if (query == null) query = new QueryManager(RepositoryManager.Default);
        //        return query;
        //    }
        //}

        ///// <summary>
        ///// 获取字符串的本地化版本
        ///// </summary>
        ///// <param name="key">字符串的 key</param>
        ///// <returns>本地化版本的字符串</returns>
        //public string L(string key)
        //{
        //    return this.Localization.Localize(key);
        //}

        ///// <summary>
        ///// 获取字符串的本地化版本
        ///// </summary>
        ///// <param name="key">字符串的 key</param>
        ///// <param name="args">参数列表</param>
        ///// <returns>本地化版本的字符串</returns>
        //public string L(string key, params object[] args)
        //{
        //    return this.Localization.Localize(key, args);
        //}

        ///// <summary>
        /////  Gets the virtual path of the directory that contains the application hosted
        /////     in the current application domain.
        ///// </summary>
        //protected internal static readonly string WebRoot = HttpRuntimeEx.Default.WebRoot ?? "/";
        ////readonly ISecurityService securityService = IoC.Default.Resolve<ISecurityService>();

        ///// <summary>
        ///// 此类型的日志记录器。
        ///// </summary>
        //protected internal static readonly Rhythm.Logging.ILogger Logger =
        //    Rhythm.Logging.Log.For(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //new protected internal NewtonsoftJsonResult Json(object obj, JsonRequestBehavior behavior)
        //{
        //    return new NewtonsoftJsonResult(obj);
        //}

        //new protected internal NewtonsoftJsonResult Json(object obj)
        //{
        //    return new NewtonsoftJsonResult(obj);
        //}

        //protected internal NewtonsoftJsonResult Json(object obj, Newtonsoft.Json.JsonSerializerSettings settings)
        //{
        //    return new NewtonsoftJsonResult(obj, settings);
        //}

        /// <summary>
        /// 获取管理后台的配置信息
        /// </summary>
        //protected internal ManagementConfiguration Configuration { get { return managementConfiguration; } }
        protected internal AjaxActionResult CreateAjaxResult()
        {
            return new AjaxActionResult(this.ControllerContext, this.ModelState)
            {
                ItemsSerializerSettings = JsonUtility.Settings.CamelCase,
                ModelStateSerializerSettings = JsonUtility.Settings.Default,
            };
        }

        /// <summary>
        /// 表示当前操作执行成功
        /// </summary>
        /// <param name="message">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        protected internal AjaxActionResult AjaxResult(string message = null)
        {
            return ModelState.IsValid ? CreateAjaxResult().Succeed(message) : CreateAjaxResult().Failure(message);
        }

        /// <summary>
        /// 表示当前操作执行成功
        /// </summary>
        /// <returns>AjaxResult</returns>
        [Obsolete("请使用 Success 代替")]
        protected internal AjaxActionResult AjaxSuccess()
        {
            return CreateAjaxResult().Succeed();
        }

        /// <summary>
        /// 表示当前操作执行成功
        /// </summary>
        /// <param name="message">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        [Obsolete("请使用 Success 代替")]
        protected internal AjaxActionResult AjaxSuccess(string message)
        {
            return CreateAjaxResult().Succeed(message);
        }

        /// <summary>
        /// 表示当前操作执行失败
        /// </summary>
        /// <returns>AjaxResult</returns>
        [Obsolete("请使用 Failure 代替")]
        protected internal AjaxActionResult AjaxFailure()
        {
            return CreateAjaxResult().Failure();
        }

        /// <summary>
        /// 表示当前操作执行失败
        /// </summary>
        /// <param name="message">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        [Obsolete("请使用 Failure 代替")]
        protected internal AjaxActionResult AjaxFailure(string message)
        {
            return CreateAjaxResult().Failure(message);
        }


        /// <summary>
        /// 表示当前操作执行成功
        /// </summary>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult Success()
        {
            return CreateAjaxResult().Succeed();
        }


        /// <summary>
        /// 表示当前操作执行成功
        /// </summary>
        /// <param name="message">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult Success(string message)
        {
            return CreateAjaxResult().Succeed(message);
        }

        /// <summary>
        /// 表示当前操作执行失败
        /// </summary>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult Failure()
        {
            return CreateAjaxResult().Failure();
        }

        /// <summary>
        /// 表示当前操作执行失败
        /// </summary>
        /// <param name="message">用于在用户界面上显示的提示消息</param>
        /// <returns>AjaxResult</returns>
        public AjaxActionResult Fail(string message)
        {
            return CreateAjaxResult().Failure(message);
        }

        protected internal AjaxActionResult AjaxRedirect(string url)
        {
            return CreateAjaxResult().Succeed().RedirectUrl(url);
        }

        /// <param name="actionName">The name of the action method.</param>
        protected internal AjaxActionResult AjaxRedirectAction(string actionName)
        {
            return CreateAjaxResult().Succeed().RedirectUrl(Url.Action(actionName));
        }

        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>AjaxResult</returns>
        protected internal AjaxActionResult AjaxRedirectAction(string actionName, string controllerName)
        {
            return CreateAjaxResult().Succeed().RedirectUrl(Url.Action(actionName, controllerName));
        }

        //protected internal HttpUnauthorizedResult HttpUnauthorized()
        //{
        //    return new HttpUnauthorizedResult();
        //}

        //protected internal HttpUnauthorizedResult HttpUnauthorized(string statusDescription)
        //{
        //    return new HttpUnauthorizedResult(statusDescription);
        //}

        public ActionResult Error(string errorMessage)
        {
            return View("Error", (object) errorMessage);
        }

        public ActionResult Error(Exception ex)
        {
            return View("Error", ex);
        }


        #region Authorization

        ///// <summary>
        ///// 判断是否有权限
        ///// </summary>
        ///// <param name="actionDef"></param>
        ///// <returns></returns>
        //public bool HasAuth<TResource>(IOperation<TResource> actionDef)
        //{
        //    if (Acc == null) return false;
        //    return Acc.HasAuth(this.WorkContext, actionDef);
        //}


        ///// <summary>
        ///// 判断是否有权限
        ///// </summary>
        ///// <param name="actionDef"></param>
        ///// <param name="state"></param>
        ///// <returns></returns>
        //public bool HasAuth(IOperation actionDef)
        //{
        //    if (Acc == null) return false;
        //    return Acc.HasAuth(this.WorkContext, actionDef);
        //}

        ///// <summary>
        ///// 判断是否有权限
        ///// </summary>
        ///// <param name="actionDefs"></param>
        ///// <returns></returns>
        //public bool HasAuth(params IOperation[] actionDefs)
        //{
        //    if (actionDefs == null || actionDefs.Length == 0) return true;
        //    if (Acc == null) return false;
        //    return Acc.HasAuth(this.WorkContext, actionDefs);
        //}


        ///// <summary>
        ///// 判断是否有权限（任意）
        ///// </summary>
        ///// <param name="actionDefs"></param>
        ///// <returns></returns>
        //public bool HasAuthAny(params IOperation[] actionDefs)
        //{
        //    if (actionDefs == null || actionDefs.Length == 0) return true;
        //    if (Acc == null) return false;
        //    return Acc.HasAuthAny(this.WorkContext, actionDefs);
        //}


        //Laz<AccessControlManager<FeatureAuthorization>> accessControl;

        //public AccessControlManager<FeatureAuthorization> AccessControl
        //{
        //    get
        //    {
        //        if (accessControl.HasValue == false)
        //        {
        //            accessControl = AccessControlManager<FeatureAuthorization>.Instance ??
        //                            IoC.Default.Resolve<AccessControlManager<FeatureAuthorization>>();
        //        }

        //        return accessControl.Value;
        //    }
        //    set { accessControl = value; }
        //}


        //Laz<FeatureAuthorizationHelper> acc;

        //FeatureAuthorizationHelper Acc
        //{
        //    get
        //    {
        //        if (acc.HasValue == false)
        //        {
        //            var user = this.User;
        //            if (user == null)
        //            {
        //                acc = new Laz<FeatureAuthorizationHelper>();
        //            }
        //            else
        //            {
        //                acc = AccessControl.CreateUserFeatureAuthorizationHelper(user);
        //            }
        //        }

        //        return acc.Value;
        //    }
        //    set { acc = value; }
        //}

        #endregion


        //WorkContext workContext;

        ///// <summary>
        ///// 当前用户、组织相关信息上下文对象
        ///// </summary>
        //public WorkContext WorkContext
        //{
        //    get
        //    {
        //        // work context can default auto set by IOC
        //        if (workContext == null) this.workContext = this.HttpContext.WorkContext();
        //        return workContext;
        //    }
        //    set { workContext = value; }
        //}


        //new public User User { get; private set; }
        //Laz<User> user;

        ///// <summary>
        ///// 用户信息
        ///// </summary>
        //new protected internal User User
        //{
        //    get
        //    {
        //        if (user.HasValue == false)
        //        {
        //            user = WorkContext.User();
        //        }

        //        return user.Value;
        //    }
        //    private set { user = value; }
        //}


        LocalizationManager localizationManager;

        /// <summary>
        /// LocalizationManager is auto set by IOC
        /// </summary>
        //[FromServices]
        public LocalizationManager LocalizationManager
        {
            get { return localizationManager; }
            set { localizationManager = value; }
        }

        //if (localizationManager == null)
        //       {
        //           localizationManager = IoC.Default.Resolve<LocalizationManager>();
        //       }

        //Rhythm.Globalization.Localization localization;

        //protected Rhythm.Globalization.Localization Localization
        //{
        //    get
        //    {
        //        if (localization == null)
        //        {
        //            localization = LocalizationManager.Resolve(this.WorkContext, this.GetType().GetNonProxyType());
        //        }

        //        return localization;
        //    }
        //    set { localization = value; }
        //}


        Rhythm.Globalization.LocalizationProvider localizationProvider;

        protected Rhythm.Globalization.LocalizationProvider LocalizationProvider
        {
            get
            {
                if (localizationProvider == null)
                {
                    localizationProvider =
                        new LocalizationProvider(LocalizationManager, this.GetType().GetNonProxyType());
                }

                return localizationProvider;
            }
            set { localizationProvider = value; }
        }

        protected override void Dispose(bool disposing)
        {
            //_repositoryContext?.Dispose();
            //// set object to null to avoid memory leak (another outer out of controled Object keep this Object)
            //this.workContext = null;
            //this.User = null;
            base.Dispose(disposing);
        }

    }
}