namespace MnLab.PdfVisualDesign.Binding.Elements
{
    public interface IValueBindGridData
    {
        ValueDef[][] AllCellValues { get; set; }
        string DisplayType { get; set; }
        string GridType { get; set; }
        MergedCell[] MergedCells { get; set; }
    }
}