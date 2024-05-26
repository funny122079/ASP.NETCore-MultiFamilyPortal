using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Layouts.Sections
{
    public partial class UtilizeMobileMenu
    {
        [CascadingParameter]
        private IPortalTheme Theme { get; set; }
        private IMenuProvider MenuProvider => Theme as IMenuProvider;
    }
}