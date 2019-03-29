using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;

namespace MnLab.PdfVisualDesign {

    public class FieldBindingInfo : IValueBindingInfo {
        public string ContentPartName { get; set; }
        public string MemberExpression { get; set; }
        public string DefaultValue { get; set; }
        public LocalizedString Description { get; set; }
        public string Remark { get; set; }
    }
}