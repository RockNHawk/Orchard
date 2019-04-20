/*************************************************
  Author:   Aska Li 664856248@qq.com       
 
  History:        
    1. Date: 2013/08/12
       Author: Aska Li
       Modification:  Service 基类。
*************************************************/
using System;

namespace Rhythm
{
    //using Rhythm.Reflection;
    //using Rhythm.ErrorHandling;
    //using Rhythm.Globalization;

    /// <summary>
    /// 业务逻辑层基类。
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// 此类型的日志记录器。
        /// </summary>
        //protected readonly Rhythm.Logging.ILogger Logger;
        //LocalizationProvider localizationProvider;
        //protected LocalizationProvider LocalizationProvider
        //{
        //    get
        //    {
        //        if (localizationProvider == null)
        //        {
        //            this.localizationProvider = new LocalizationProvider(LocalizationManager, serviceType);
        //        }
        //        return localizationProvider;
        //    }
        //    set { localizationProvider = value; }
        //}
        //public LocalizationManager LocalizationManager { get; set; }

        //public IoC Depedency { get; set; } = IoC.Default;

        readonly System.Type serviceType;
        /// <summary>
        /// 受保护的构造函数
        /// </summary>
        protected ServiceBase()
        {
            //var type = this.GetType();
            //serviceType = type.IsProxyType() ? type.BaseType : type;
            //if (Logger == null) Logger = Rhythm.Logging.Log.For(type);
        }

        ///// <summary>
        ///// 获取字符串的本地化版本（推荐使用第一个参数为 IContext 的重载版本）
        ///// </summary>
        ///// <param name="key">字符串的 key</param>
        ///// <returns>本地化版本的字符串</returns>
        //[Obsolete("推荐使用第一个参数为 IContext 的重载版本")]
        //public string L(string key)
        //{
        //    return this.LocalizationProvider.Localize(key);
        //}

        ///// <summary>
        ///// 获取字符串的本地化版本（推荐使用第一个参数为 IContext 的重载版本）
        ///// </summary>
        ///// <param name="key">字符串的 key</param>
        ///// <param name="args">参数列表</param>
        ///// <returns>本地化版本的字符串</returns>
        //[Obsolete("推荐使用第一个参数为 IContext 的重载版本")]
        //public string L(string key, params object[] args)
        //{
        //    return this.LocalizationProvider.Localize(key, args);
        //}

        ///// <summary>
        ///// 获取字符串的本地化版本
        ///// </summary>
        ///// <param name="context">当前上下文</param>
        ///// <param name="key">字符串的 key</param>
        ///// <returns>本地化版本的字符串</returns>
        //public string L(IContext context, string key)
        //{
        //    return this.LocalizationProvider.Localize(context, key);
        //}

        ///// <summary>
        ///// 获取字符串的本地化版本
        ///// </summary>
        ///// <param name="context">当前上下文</param>
        ///// <param name="key">字符串的 key</param>
        ///// <param name="args">参数列表</param>
        ///// <returns>本地化版本的字符串</returns>
        //public string L(IContext context, string key, params object[] args)
        //{
        //    return this.LocalizationProvider.Localize(context, key, args);
        //}


        /// <summary>
        /// 在此业务层中抛出一个验证失败的错误信息。
        /// </summary>
        /// <typeparam name="TException">异常的类型</typeparam>
        /// <param name="ex">异常信息对象</param>
        /// <returns>异常信息对象，你仍需要 throw</returns>
        protected static TException ValidationError<TException>(TException ex) where TException : System.Exception
        {
            //ExcepionHandlingExtensions.SetValidationSeverity(ex);
            return ex;
        }

        /// <summary>
        /// 在此业务层中抛出一个验证失败的错误信息。
        /// </summary>
        protected static Exception ValidationError(string message)
        {
            return new Exception(message);
           // return ExcepionHandlingExtensions.SetValidationSeverity(new BusinessException(message));
        }

        /// <summary>
        /// 在此业务层中抛出一个验证失败的错误信息。
        /// </summary>
        protected static Exception ValidationError(string message, Exception innerException)
        {
            return innerException;
          //  return ExcepionHandlingExtensions.SetValidationSeverity(new BusinessException(message, innerException));
        }

        ///// <summary>
        ///// 在此业务层中抛出一个验证失败的错误信息。
        ///// </summary>
        //protected static BusinessException ValidationModelStateError(string key, string message)
        //{
        //    return ExceptionFactory.ModelState(key, message);
        //}

        ///// <summary>
        ///// 在此业务层中抛出一个验证失败的错误信息。
        ///// (注意：propertyName 会被忽略，这个方法没有全部实现)
        ///// </summary>
        //protected static BusinessException ValidationModelStateError(string key, string message, Exception innerException)
        //{
        //    return ExceptionFactory.ModelState(key, message, innerException);
        //}

        /// <summary>
        /// 在此业务层中抛出一个验证失败的错误信息。
        /// </summary>
        protected static Exception ValidationEntityNotExistsError<TEntity>(int entityId) where TEntity : class {
            return new InvalidOperationException("Entity " + typeof(TEntity).FullName + "#" + entityId + "不存在。");
            //return ExcepionHandlingExtensions.SetValidationSeverity(new InvalidOperationException("Entity " + typeof(TEntity).FullName + "#" + entityId + "不存在。"));
        }

        ///// <summary>
        ///// 在此业务层中抛出一个验证失败的错误信息。
        ///// </summary>
        //protected static Exception ValidationEntityNotExistsError<TEntity>(Guid entityId) where TEntity : class
        //{
        //    return ExcepionHandlingExtensions.SetValidationSeverity(new InvalidOperationException("Entity " + typeof(TEntity).FullName + "#" + entityId + "不存在。"));
        //}

        ///// <summary>
        ///// 在此业务层中抛出一个验证失败的错误信息。
        ///// </summary>
        //protected static Exception ValidationEntityNotExistsError<TEntity>(long entityId) where TEntity : class
        //{
        //    return ExcepionHandlingExtensions.SetValidationSeverity(new InvalidOperationException("Entity " + typeof(TEntity).FullName + "#" + entityId + "不存在。"));
        //}

        ///// <summary>
        ///// 在此业务层中抛出一个验证失败的错误信息。
        ///// </summary>
        //protected static Exception ValidationEntityNotExistsError<TEntity>(object entityId) where TEntity : class
        //{
        //    if (entityId == null)
        //    {
        //        throw new ArgumentNullException(nameof(entityId));
        //    }
        //    return ExcepionHandlingExtensions.SetValidationSeverity(new InvalidOperationException("Entity " + typeof(TEntity).FullName + "#" + entityId + "不存在。"));
        //}

        /// <summary>
        /// 向事件总线发布事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgs"></param>
        internal protected static void PublishEvent<TEvent>(TEvent eventArgs)
        {
            EventBus.Default.Publish(eventArgs);
        }

    }
}
