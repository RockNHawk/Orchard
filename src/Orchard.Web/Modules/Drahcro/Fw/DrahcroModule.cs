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
using Drahcro.Data;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Mvc.Extensions;
using Orchard.Mvc.Routes;
using Orchard.Settings;

namespace Drahcro {

    public class DrahcroModule : Module {

        protected override void Load(ContainerBuilder builder) {
            builder.RegisterGeneric(typeof(ContentPartRepository<,>)).As(typeof(IContentPartRepository<,>));
        }
    }
}
