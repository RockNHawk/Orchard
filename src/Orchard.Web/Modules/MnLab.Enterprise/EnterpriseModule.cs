using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Instrumentation;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using MnLab.Enterprise.Approval;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Routes;
using Orchard.Settings;

namespace MnLab.Enterprise {

    //[OrchardSuppressDependency("Orchard.Layouts.Framework.Display.ElementDisplay")]
    public class EnterpriseModule : Module {

        protected override void Load(ContainerBuilder builder) {
            //moduleBuilder.RegisterType<CustomElementDisplay >().As<IElementDisplay>();
           // builder.RegisterGeneric(typeof(ContentPartRepository<,>)).As(typeof(IContentPartRepository<,>));
        }

    }




    //public class SimpleInterceptor : IInterceptor {
    //    public void Intercept(IInvocation invocation) {
    //        if (invocation.Method.Name == "SimpleMethod") {
    //            invocation.ReturnValue = "different return value";
    //        }
    //        else {
    //            invocation.Proceed();
    //        }
    //    }
    //}


}
