namespace Rhythm.Web
{

    public class HttpApplicationEx
    {
        public static string AppDomainAppVirtualPath
        {
            get { return HttpRuntimeEx.AppDomainAppVirtualPath; }
            set { HttpRuntimeEx.AppDomainAppVirtualPath = value; }
        }

        public static string WebRoot { get { return HttpRuntimeEx.WebRoot; } }
    }

}