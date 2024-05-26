using MultiFamilyPortal.Themes;

#nullable disable
namespace MultiFamilyPortal.SuspendedTenantTheme
{
    public class SuspendedTenantTheme : IPortalTheme
    {
        public Type Layout { get; } = typeof(Layouts.MainLayout);
        public string Name { get; } = "Suspended";
        public Type _404 { get; }
        public string[] RequiredStyles { get; } = new[] { "_content/MultiFamilyPortal.SuspendedTenantTheme/css/site.css" };
    }
}
#nullable restore
