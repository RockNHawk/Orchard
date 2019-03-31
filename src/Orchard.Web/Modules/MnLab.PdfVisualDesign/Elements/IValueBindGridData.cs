namespace MnLab.PdfVisualDesign.Binding.Elements
{
    public interface IValueBindGridData
    {
        ValueBindingDef[][] AllCellValues { get; set; }
        string DisplayType { get; set; }
        string GridType { get; set; }
        MergedCell[] MergedCells { get; set; }
    }
}