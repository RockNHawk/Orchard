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
using Orchard.Layouts.Framework.Display;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Routes;
using Orchard.Settings;

namespace MnLab.PdfVisualDesign {
    public class MvcModule : Module {
        public const string IsBackgroundHttpContextKey = "IsBackgroundHttpContext";

        protected override void Load(ContainerBuilder moduleBuilder) {
            moduleBuilder.RegisterType<CustomElementDisplay >().As<IElementDisplay>();
            //moduleBuilder.RegisterType<ShellRoute>().InstancePerDependency();
        }
 
    }
}
