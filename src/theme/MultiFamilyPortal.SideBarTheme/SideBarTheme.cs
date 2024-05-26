using System.Reflection;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.SideBarTheme
{
    public abstract class SideBarTheme : IPortalTheme, IApplicationPart, IPortalMenuProvider
    {
        protected SideBarTheme()
        {
            Assemblies = new[] { GetType().Assembly };
            RequiredStyles = new[] 
            {
                "_content/MultiFamilyPortal.SideBarTheme/css/site.css",
                "_content/MultiFamilyPortal.CoreUI/css/responsive.css" 
            };
        }

        public Type Layout { get; } = typeof(Layouts.MainLayout);
        public abstract string Name { get; }
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; protected set; } 
        public IEnumerable<Assembly> Assemblies { get; protected set; }
        public abstract Type SideBar { get; }
    }
}
