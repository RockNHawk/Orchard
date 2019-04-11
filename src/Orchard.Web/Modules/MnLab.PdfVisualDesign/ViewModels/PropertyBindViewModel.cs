using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MnLab.PdfVisualDesign.Binding.Elements;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Services;

namespace MnLab.PdfVisualDesign.ViewModels {

    //public class PropertyBindValueViewModel {
    //    public string Value { get; set; }
    //}


    public class PropertyBindViewModel  {

        public IContent Content { get; set; }

        public ContentField Field { get; set; }

        public dynamic EditorShape { get; set; }

        //public IEnumerable<DriverResult> FieldDrivers { get; set; }
        public IContentFieldDisplay FieldDisplay { get; set; }

        public ValueDef Binding { get; set; }
        public ElementEditorContext Context { get;  set; }

        //public object Value { get; set; }

        //public IList<string> Properties { get; set; }


    }
}