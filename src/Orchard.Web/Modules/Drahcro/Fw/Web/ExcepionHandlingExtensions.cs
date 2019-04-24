using System;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;
using System.Web.Mvc;
using System.Globalization;
using System.Threading;
using Rhythm.ErrorHandling;

namespace Drahcro.ErrorHandling
{
	/// <summary>
	/// 提供对异常处理的扩展方法
	/// </summary>
	public static class ExcepionHandlingExtensions
    {
       	const string key = "_rhythm_info";


		/// <summary>
		/// 设置 异常 Id
		/// <para></para>
		/// 一些异常不应该在界面上显示出来，而是显示一个介绍性的描述如：核查身份时连接第三方系统失败，错误编号 :201401221 ( log id ) ，用户反馈的时候，报告这个 ID ，根据这个 ID 去追溯异常
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ex"></param>
		/// <param name="exceptionId"></param>
		/// <returns></returns>

		/// <summary>
		/// 获取 异常 Id
		/// <para></para>
		/// 一些异常不应该在界面上显示出来，而是显示一个介绍性的描述如：核查身份时连接第三方系统失败，错误编号 :201401221 ( log id ) ，用户反馈的时候，报告这个 ID ，根据这个 ID 去追溯异常
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>

		/// <summary>
		/// 获取异常的 Model State
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static ModelStateDictionary GetModelState(this Exception ex)
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return null;
			return info.ModelState;
		}

		/// <summary>
		/// 往异常中增加一个 Model State
		/// </summary>
		/// <typeparam name="T">异常的类型</typeparam>
		/// <param name="ex">异常对象</param>
		/// <param name="key">Model State 的 key，通常为属性名</param>
		/// <param name="exception">错误信息</param>
		/// <returns></returns>
		public static T AddModelState<T>(this T ex, string key, Exception exception) where T : System.Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			var modelState = info.ModelState ?? (info.ModelState = new ModelStateDictionary());
			modelState.AddModelError(key, exception);
			//List<Exception> exceptions;
			//if (!modelState.TryGetValue(key, out exceptions))
			//{
			//	exceptions = new List<Exception>(1);
			//	modelState[key] = exceptions;
			//}
			//exceptions.Add(exception);
			return ex;
		}


		/// <summary>
		/// 往异常中增加一个 Model State
		/// </summary>
		/// <typeparam name="T">异常的类型</typeparam>
		/// <param name="ex">异常对象</param>
		/// <param name="key">Model State 的 key，通常为属性名</param>
		/// <param name="errorMessage">错误信息</param>
		/// <returns></returns>
		public static T AddModelState<T>(this T ex, string key, string errorMessage) where T : System.Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			var modelState = info.ModelState ?? (info.ModelState = new ModelStateDictionary());
			modelState.AddModelError(key, errorMessage);
			return ex;
		}

		/// <summary>
		/// 设置 CustomError Message，用于在用户界面上显示
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ex"></param>
		/// <param name="customErrorMessage"></param>
		/// <returns></returns>
		public static T SetCustomErrorMessage<T>(this T ex, string customErrorMessage) where T : System.Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.CustomErrorMessage = customErrorMessage;
			return ex;
		}

		/// <summary>
		/// 获取 CustomError Message，用于在用户界面上显示
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string GetCustomErrorMessage(this Exception ex)
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return null;
			return info.CustomErrorMessage;
		}

		/// <summary>
		/// 设置异常的级别
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static T SetSeverity<T>(this T ex, ExceptionSeverity sv) where T : System.Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.Severity = sv;
			return ex;
		}



		/// <summary>
		/// 获取异常的级别
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static T SetErrorSeverity<T>(this T ex) where T : System.Exception
		{
			return SetSeverity(ex, ExceptionSeverity.Error);
		}

		/// <summary>
		/// 设置异常的级别
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static T SetValidationSeverity<T>(this T ex) where T : System.Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			return SetSeverity(ex, ExceptionSeverity.Validation);
		}

		/// <summary>
		/// 获取异常的级别
		/// </summary>
		/// <typeparam name="TException"></typeparam>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static ExceptionSeverity GetSeverity<TException>(this TException ex) where TException : System.Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return default(ExceptionSeverity);
			return info.Severity;
		}

		/// <summary>
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <returns>如果层级被 catch 过了则返回True。</returns>
		public static bool IsFirstCatched(this Exception ex)
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return false;
			return info.IsFirstCatched;
		}

		/// <summary>
		/// 标记异常为第一次被捕获。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		public static TException MarkAsFirstCatched<TException>(this TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.IsFirstCatched = true;
			return ex;
		}

		/// <summary>
		/// 向异常中添加方法的调用跟踪信息。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		public static TException AddInvocationTrace<TException>(this TException ex, string traceInfo) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.InvocationTraceKey =
			 (info.InvocationTraceKey == null ? "" : info.InvocationTraceKey + "\r\n") + traceInfo;
			return ex;
		}


		/// <summary>
		/// 向异常中添加方法的调用跟踪信息。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		public static TException AddInvocationTrace<TException>(this TException ex, StringBuilder traceInfo) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.InvocationTraceKey =
				    (info.InvocationTraceKey == null ? "" : info.InvocationTraceKey + "\r\n") + traceInfo.ToString();
			return ex;
		}

		/// <summary>
		/// 获取此异常的方法的调用跟踪信息。
		/// </summary>
		/// <param name="ex">异常对象></param>
		/// <returns>此异常的方法的调用跟踪信息，可能为 null。</returns>
		public static string GetInvocationTrace(this Exception ex)
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return null;
			return info.InvocationTraceKey;
		}


		/// <summary>
		/// 获取此异常是否已经被记录过日志了。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <returns>如果已经被记录过了则返回True。</returns>
		public static bool HasAlreadyLogged(this Exception ex)
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return false;
			return info.AlreadyLoggedKey;
		}

		/// <summary>
		/// 标记此异常为已经被记录在日志了。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		public static TException MarkAsAlreadyLogged<TException>(this TException ex ) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.AlreadyLoggedKey = true;
			return ex;
		}

		/// <summary>
		/// 获取此异常是否已经被任何一个Exception Handler处理过了。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <returns>如果已经被处理过了则返回True。</returns>
		public static bool HasAlreadyHandled(this Exception ex)
		{
			var info = ex.GetExtraInfo();
			if (info == null) return false;
			return info.AlreadyHandledKey;
		}

		/// <summary>
		/// 标记此异常为已经被处理过。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <param name="handledBy">对已经处理的描述信息，可以写被什么方式处理。</param>
		public static TException MarkAsAlreadyHandled<TException>(this TException ex, string handledBy) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.AlreadyHandledKey = true;
			info.AlreadyHandledBy = handledBy;
			return ex;
		}


		public static TException SetExceptionId<TException>(this TException ex, string exceptionId) where TException : Exception
		{
			var info = ex.GetOrSetExtraInfo();
			info.ExceptionId = exceptionId;
			return ex;
		}

		public static string GetExceptionId<TException>(this TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return null;
			return info.ExceptionId;
		}

		static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		
		static int s_SeqNo;
		public static string GetOrGenerateExceptionId<TException>(this TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			if (info.ExceptionId == null) { 
				int num = Interlocked.Increment(ref s_SeqNo);
				return	info.ExceptionId  = DateTime.Now.ToString("yyyyMMddHHmmss", invariantCulture) + num.ToString(invariantCulture);
			}
			return info.ExceptionId;
		}

	 

		public static TException SetAssembly<TException>(this TException ex, Assembly assembly) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.Assembly = assembly;
			return ex;
		}

		public static TException SetType<TException>(this TException ex, Type type) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetOrSetExtraInfo();
			info.Type = type;
			return ex;
		}

		public static Type GetType<TException>(TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return null;
			return info.Type;
		}

		public static Assembly GetAssembly<TException>(TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			var info = ex.GetExtraInfo();
			if (info == null) return null;
			return info.Assembly;
		}

		public static TException SetExtraInfo<TException>(this TException ex, ExceptionInfo info) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			ex.Data[key] = info;
			return ex;
		}

		public static ExceptionInfo GetExtraInfo<TException>(this TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			return ex.Data.Contains(key) ? (ExceptionInfo)ex.Data[key] : null;
		}

		public static ExceptionInfo GetOrSetExtraInfo<TException>(this TException ex) where TException : Exception
		{
			if (ex == null) throw new ArgumentNullException(nameof(ex));
			if (ex.Data.Contains(key))
			{
				return (ExceptionInfo)ex.Data[key];
			}
			var info = new ExceptionInfo();
			ex.Data[key] = info;
			return info;
		}

    }
}
