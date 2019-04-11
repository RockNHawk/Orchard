using Orchard.Localization;

namespace MnLab.PdfVisualDesign {
    public interface IValueDef {
        string BindType { get; set; }
       string Remark { get; set; }

    }

    public interface IStaticValueDef : IValueDef {
          string StaticValue { get; set; }
    }

    public interface IValueBindingDef : IValueDef {

        string ContentPartName { get; set; }
        string MemberExpression { get; set; }
        string DefaultValue { get; set; }

        string Key { get; }

        string DisplayName { get; }

        //LocalizedString Description { get; set; }
    }
}