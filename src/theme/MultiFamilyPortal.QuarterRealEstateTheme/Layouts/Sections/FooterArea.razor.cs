using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Layouts.Sections
{
    public partial class FooterArea
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default!;

        [CascadingParameter]
        public ClaimsPrincipal User { get; set; } = default!;

        [Inject]
        private IFormService _formService { get; set; } = default!;

        private PortalNotification notification { get; set; } = default!;
        private ServerSideValidator serverSideValidator { get; set; } = default!;

        private NewsletterSubscriberRequest SignupModel => _formService?.SignupModel;
        private bool subscribed;
        private async Task OnValidSignupRequest(EditContext context)
        {
            var response = await _formService.SubmitSubscriberSignup();

            if(response?.Errors?.Any() ?? false)
            {
                serverSideValidator.DisplayErrors(response.Errors);
            }

            notification.Show(response);
            subscribed = response.State == ResultState.Success;
        }
    }
}
