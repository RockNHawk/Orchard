using Orchard.Localization;

namespace MnLab.PdfVisualDesign {
    public interface IValueBindingDef {
        string Key { get; }

        string ContentPartName { get; set; }
        string MemberExpression { get; set; }

        string DefaultValue { get; set; }
        //LocalizedString Description { get; set; }
        string Remark { get; set; }
    }
}