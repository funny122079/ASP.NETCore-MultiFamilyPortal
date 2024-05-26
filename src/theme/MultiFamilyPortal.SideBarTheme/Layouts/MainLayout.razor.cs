using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.SideBarTheme.Layouts
{
    public partial class MainLayout
    {
        [CascadingParameter]
        private IPortalTheme Theme { get; set; } = default!;

        private Type? SideBar;

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; } = default!;

        private ClaimsPrincipal User = new ClaimsPrincipal();
        protected override async Task OnInitializedAsync()
        {
            if (Theme is IPortalMenuProvider menuProvider)
                SideBar = menuProvider.SideBar;

            var authState = await authenticationStateTask;
            User = authState.User;
        }
    }
}
