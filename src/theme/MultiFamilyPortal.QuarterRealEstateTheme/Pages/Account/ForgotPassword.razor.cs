using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages.Account
{
    public partial class ForgotPassword
    {
        private ForgotPasswordRequest Input { get; set; } = new ForgotPasswordRequest();
        private string Title = "Forgot Password";

        [Inject]
        private IAuthenticationService _authenticationService { get; set; } = default !;

        private async Task OnFormSubmitted(EditContext editContext)
        {
            await _authenticationService.ForgotPassword(Input);
            Title = "Forgot password confirmation";
        }
    }
}
