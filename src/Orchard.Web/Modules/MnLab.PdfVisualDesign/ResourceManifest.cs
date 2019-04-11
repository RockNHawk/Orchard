using Orchard.UI.Resources;

namespace MnLab.PdfVisualDesign.Binding
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();


            var url = "~/Modules/MnLab.PdfVisualDesign/Scripts/";
            var styleurl= "~/Modules/MnLab.PdfVisualDesign/Styles/";
            manifest.DefineScript("handsontable").SetUrl(url+"handsontable/handsontable.full.js");
            manifest.DefineStyle("handsontable").SetUrl(url+"handsontable/handsontable.css");
      
            manifest.DefineScript("inputclassEnhance").SetUrl(url + "inputclassEnhance.js");

            manifest.DefineScript("common").SetUrl(url + "common.js");
            manifest.DefineScript("handsontable_custom").SetUrl(url + "handsontable_custom.js");
            manifest.DefineScript("tbales").SetUrl(url + "table.js");
            manifest.DefineStyle("table").SetUrl(styleurl + "table.css");

        }
    }
}