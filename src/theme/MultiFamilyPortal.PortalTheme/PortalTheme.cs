using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.PortalTheme
{
    public class PortalTheme : IPortalFrontendTheme, IScriptProvider, IBodyClassProvider
    {
        public Type Layout { get; } = typeof(Layouts.MainLayout);
        public string Name { get; } = "Portal Theme";
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[]
        {
            "_content/MultiFamilyPortal.PortalTheme/css/site.css",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/css/animate.css",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/css/style.css",
        };
        public string[] Scripts { get; } = new[]
        {
            "https://cdnjs.cloudflare.com/ajax/libs/popper.js/2.10.2/umd/popper.min.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/plugins/metisMenu/jquery.metisMenu.js",
            //"https://cdn.avantipoint.com/themes/inspinia/2.9.4/js/plugins/metisMenu/jquery.metisMenu.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/plugins/slimscroll/jquery.slimscroll.min.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/inspinia.js",
            "https://avantipoint.blob.core.windows.net/theme/inspinia/2.9.4/js/plugins/pace/pace.min.js",
        };
        string IBodyClassProvider.Class { get; } = "gray-bg";

        public ThemeResource[] Resources { get; } = Array.Empty<ThemeResource>();
    }
}
