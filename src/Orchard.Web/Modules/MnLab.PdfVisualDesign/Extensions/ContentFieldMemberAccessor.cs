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

namespace MnLab.PdfVisualDesign.Binding.Drivers {
    [System.Diagnostics.DebuggerDisplay("{memberName}=_memberName,{field}=_field,{ContentPart}=_contentPart")]
    public class ContentPartFieldMemberAccessor : IContentPartMemberAccessor {
        string _memberName;
        ContentField _field;
        ContentPart _contentPart;
        public ContentPartFieldMemberAccessor(ContentPart contentPart, ContentField field, string memberName) {
            this._contentPart = contentPart;
            this._field = field;
            this._memberName = memberName;
            /*
            这个方法不是100%保险的，因为存在有的 Field 有一个叫做 Value 的 Proeprty 但是还有其它 Prop 的情况
             */
            this.IsSimply = field.GetType().GetProperty("Value") != null;
        }

        public bool IsSimply { get; set; }

        public object GetObject() {
            return _field;
        }

        public object GetValue(string name = null) {

            /**
             *
这里 name 传 null，似乎获取的是 Field 的默认属性"Value"的值，如果存在多个属性，就获取有问题了
    public class TextField : ContentField {
        public string Value {
            get { return Storage.Get<string>(); }
            set { Storage.Set(value); }
        }
    }
             */
            return _field.Storage.Get<object>(name);
        }



        public void SetValue(object value) {
            _field.Storage.Set<object>(null, value);
        }
    }
}


