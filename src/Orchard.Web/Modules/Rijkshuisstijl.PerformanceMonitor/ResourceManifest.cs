using Orchard.UI.Resources;

namespace Rijkshuisstijl.PerformanceMonitor
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            Orchard.UI.Resources.ResourceManifest manifest = builder.Add();
            manifest.DefineScript("DataTables").SetUrl("jquery.dataTables.min.js").SetCdn("http://ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/jquery.dataTables.min.js").SetDependencies("jQuery");
            manifest.DefineStyle("DataTables").SetUrl("jquery.dataTables.css").SetCdn("http://ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/css/jquery.dataTables.css");

            manifest.DefineScript("jquerymin").SetUrl("jquery.min.js");
            manifest.DefineScript("jqplot").SetUrl("jquery.jqplot.min.js").SetDependencies("jQuery");
            manifest.DefineScript("excanvas").SetUrl("excanvas.js").SetDependencies("jQuery");
            manifest.DefineStyle("jqplot").SetUrl("jquery.jqplot.css");

            manifest.DefineScript("jqplotpointlabels").SetUrl("jqplot.pointLabels.min.js").SetDependencies("jQuery");
            manifest.DefineScript("jqplotcursor").SetUrl("jqplot.cursor.min.js").SetDependencies("jQuery");
            manifest.DefineScript("jqplothighlighter").SetUrl("jqplot.highlighter.min.js").SetDependencies("jQuery");
            manifest.DefineScript("jqplotcanvasoverlay").SetUrl("jqplot.canvasOverlay.min.js").SetDependencies("jQuery");

            
        }
    }
}