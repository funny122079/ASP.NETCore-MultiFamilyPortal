using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Layouts.Sections
{
    public partial class HeaderArea
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default!;

        [CascadingParameter]
        public ClaimsPrincipal User { get; set; } = default!;

        [CascadingParameter]
        private IPortalTheme Theme { get; set; }

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        private IMenuProvider MenuProvider => Theme as IMenuProvider;

        private void AdminPortal() => _navigationManager.NavigateTo("/admin", true);

        private void InvestorPortal() => _navigationManager.NavigateTo("investor-portal", true);
    }
}
