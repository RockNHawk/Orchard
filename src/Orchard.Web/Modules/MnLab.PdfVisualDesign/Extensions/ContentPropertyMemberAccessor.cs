using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;
using System.Linq;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.FileSystems.VirtualPath;
using Orchard;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Zones;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using Orchard.Core.Title.Models;

namespace MnLab.PdfVisualDesign.Binding.Drivers
{
public class ContentPropertyMemberAccessor : IContentMemberAccessor {
        string _memberName;
        PropertyInfo _field;
        ContentPart _contentItem;
        public ContentPropertyMemberAccessor(ContentPart contentItem, PropertyInfo field, string memberName) {
            this._contentItem = contentItem;
            this._field = field;
            this._memberName = memberName;
        }

        public object GetValue() {
            return _field.GetValue(_contentItem);
        }

        public void SetValue(object value) {
            _field.SetValue(_contentItem, value);
        }
    }
}