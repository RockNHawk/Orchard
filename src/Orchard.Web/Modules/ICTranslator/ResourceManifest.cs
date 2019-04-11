namespace Orchard.UI.Resources {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("ICTranslatorCss").SetUrl("ICTranslator.css");
        }
    }
}
