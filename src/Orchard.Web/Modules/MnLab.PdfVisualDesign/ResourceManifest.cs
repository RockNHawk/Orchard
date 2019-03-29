using Orchard.UI.Resources;

namespace MnLab.PdfVisualDesign.Binding
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("handsontable").SetUrl("handsontable/handsontable.full.js");
            manifest.DefineStyle("handsontable").SetUrl("handsontable/handsontable.css");
        }
    }
}