using System;
using System.Collections.Generic;
using System.Linq;
using Rhythm.Globalization;
using System.Security.Principal;
using Rhythm.Context;
using Rhythm.EventSourcing;



#if NETFRAMEWORK
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
#else
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Diagnostics;

#endif


namespace Rhythm {
    //    using ContextValueFactory = Func<IContext, ILazyValue>;



    //    class ContextValueEntry<T> : ContextValueEntry
    //    {
    //        public new ILazyValue<T> Value => (ILazyValue<T>) base.Value;
    //    }

    public class WorkContext : ContextBase {

        //public WorkContext() {
        //}

        public WorkContext() : this(null, null, null) {
        }

        // avoid dll not found ex
        public WorkContext(HttpContext httpContext) : base(httpContext, null, nuull) {
        }

        public WorkContext(HttpContext httpContext,
            ContextDependencyManager dependencyManager = null, WorkContext parent = null)
            : base(httpContext, httpContext?.GetItems(), dependencyManager, parent) {
            //this.httpContext = httpContext;
            //this.items = httpContext?.Items ?? new Dictionary<object, object>();

            //this._dependencyManager =
            //    dependencyManager ?? parent?._dependencyManager ?? ContextDependencyManager.Default;
            //this._serviceProvider = new ContextServiceProvider(this._dependencyManager, this);

            ////this.trace = new EventTrace(this);
            //this.parent = parent;
            //InitValue();
        }



        [Newtonsoft.Json.JsonIgnore] HttpContext httpContext;

        [Newtonsoft.Json.JsonIgnore]
        public HttpContext HttpContext {
            get { return httpContext; }
            set { httpContext = value; }
        }


        public override IContext CreateChildContext() {
            return new WorkContext(this.httpContext, this._dependencyManager, this) {
                errors = errors,
            };
        }


        /// <summary>
        /// TODO:netcore
        /// </summary>
        public override bool HasError {
            get {
#if NETFRAMEWORK
                return this.httpContext.Server.GetLastError() != null;
#else
                var feature = this.HttpContext?.Features.Get<IExceptionHandlerFeature>();
                return sourceContext != null && feature?.Error != null;
#endif
            }
        }


        public static WorkContext Create() {
            return new WorkContext();
        }

    }

}