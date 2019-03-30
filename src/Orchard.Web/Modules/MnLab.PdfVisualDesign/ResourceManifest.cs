using Orchard.UI.Resources;

namespace MnLab.PdfVisualDesign.Binding
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();


            var url = "~/Modules/MnLab.PdfVisualDesign/Scripts/";
            manifest.DefineScript("handsontable").SetUrl(url+"handsontable/handsontable.full.js");
            manifest.DefineStyle("handsontable").SetUrl(url+"handsontable/handsontable.css");
        }
    }
}