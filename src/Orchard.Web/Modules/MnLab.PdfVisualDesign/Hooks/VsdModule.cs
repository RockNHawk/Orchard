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
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Routes;
using Orchard.Settings;

namespace MnLab.PdfVisualDesign {

    [OrchardSuppressDependency("Orchard.Layouts.Framework.Display.ElementDisplay")]
    public class MvcModule : Module {
        public const string IsBackgroundHttpContextKey = "IsBackgroundHttpContext";

        protected override void Load(ContainerBuilder moduleBuilder) {

            //builder.RegisterType<MyAutorouteService>().As<IAutorouteService>();

            moduleBuilder.RegisterType<CustomElementDisplay >().As<IElementDisplay>();
            //moduleBuilder.RegisterType<ShellRoute>().InstancePerDependency();


         //   moduleBuilder.RegisterType<SimpleInterceptor>();
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
