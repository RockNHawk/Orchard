using Orchard.Localization;

namespace MnLab.PdfVisualDesign
{
    public interface IValueBindingInfo
    {
        string ContentPartName { get; set; }
        string DefaultValue { get; set; }
        //LocalizedString Description { get; set; }
        string MemberExpression { get; set; }
        string Remark { get; set; }
    }
}