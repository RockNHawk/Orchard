using Orchard.UI.Resources;

namespace Pvlt.Localization {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineStyle("PvltLocalization")
                .SetUrl(
                    url: "culturepicker.min.css",
                    urlDebug: "culturepicker.css");

        }
    }
}