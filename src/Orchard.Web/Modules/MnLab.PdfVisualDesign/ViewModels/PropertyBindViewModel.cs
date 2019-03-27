using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MnLab.PdfVisualDesign.Binding.Elements;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Layouts.Services;

namespace MnLab.PdfVisualDesign.ViewModels {

    //public class PropertyBindValueViewModel {
    //    public string Value { get; set; }
    //}


    public class FieldBindingInfo {
        public string ContentPartName { get; set; }
        public string ContentPartFieldExpression { get; set; }
        public string ExampleValue { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }

    public class PropertyBindViewModel {

        public IContent Content { get; set; }

        public ContentField Field { get; set; }

        public BuildShapeContext EditorShapeContext { get; set; }

        //public IEnumerable<DriverResult> FieldDrivers { get; set; }
        public IContentFieldDisplay FieldDisplay { get; set; }

        public FieldBindingInfo Binding { get; set; }

        //public object Value { get; set; }

        //public IList<string> Properties { get; set; }


    }
}