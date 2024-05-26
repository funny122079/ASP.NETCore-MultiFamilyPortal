using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Layouts
{
    public partial class MainLayout
    {
        [Inject]
        private IWebHostEnvironment Environment { get; set; } = default!;

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; } = default!;

        private ClaimsPrincipal User = new ClaimsPrincipal();
        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            User = authState.User;
        }
    }
}
