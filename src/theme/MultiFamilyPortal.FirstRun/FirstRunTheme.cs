using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.FirstRun
{
    public class FirstRunTheme : IPortalTheme
    {
        public Type Layout { get; } = typeof(Layouts.MainLayout);
        public string Name { get; } = "First Run";
        public Type _404 { get; }
        public string[] RequiredStyles { get; } = new[] { "_content/MultiFamilyPortal.FirstRun/site.css" };
    }
}
