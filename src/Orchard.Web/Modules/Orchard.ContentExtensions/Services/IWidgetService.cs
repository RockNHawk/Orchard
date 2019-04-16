using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ContentExtensions.Services
{
    public interface IWidgetService: IDependency
    {
        void GetWidgets(dynamic model, HttpContextBase context);
    }
}