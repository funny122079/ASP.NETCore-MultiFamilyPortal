using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.CoreUI.Extensions;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.PortalTheme.Pages
{
    public partial class Index
    {
        [Inject]
        private SignInManager<SiteUser> _signinManager { get; set; } = default !;

        [Inject]
        private IJSRuntime _jSRuntime { get; set; } = default !;

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private ElementReference submitForm { get; set; }
        private string _encodedUrl => HttpUtility.UrlEncode($"/account/login?username={Input.Email}&error=1");
        private LoginRequest Input { get; set; } = new LoginRequest();
        private AuthenticationScheme _micrsoftScheme;
        private AuthenticationScheme _googleScheme;

        private ServerSideValidator serverSideValidator { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (_navigationManager.TryGetQueryString("username", out var email))
                Input.Email = email;

            var externalSchemes = await _signinManager.GetExternalAuthenticationSchemesAsync();
            if (externalSchemes?.Any() ?? false)
            {
                _micrsoftScheme = externalSchemes.FirstOrDefault(x => x.Name == "Microsoft");
                _googleScheme = externalSchemes.FirstOrDefault(x => x.Name == "Google");
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if(firstRender)
            {
                if(_user.Identity.IsAuthenticated && _user.IsInAnyRole(PortalRoles.Mentor, PortalRoles.BlogAuthor, PortalRoles.Underwriter, PortalRoles.PortalAdministrator))
                {
                    _navigationManager.NavigateTo("/admin", true);
                    return;
                }
                else if (_navigationManager.TryGetQueryString("error", out var error) &&
                    (error.Equals("1") || error.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)))
                    serverSideValidator.DisplayErrors(new Dictionary<string, List<string>>
                {
                    { string.Empty, new List<string> { "Invalid username or password." } }
                });
            }

            
        }

        private async Task SignInAsync(EditContext editContext)
        {
            await _jSRuntime.InvokeVoidAsync("MFPortal.SubmitForm", submitForm);
        }
    }
}
