using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages
{
    public partial class ContactUs
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }

        [Inject]
        private IFormService _formService { get; set; }

        private ContactFormRequest form => _formService?.ContactForm;
        private PortalNotification notification { get; set; } = default!;
        private ServerSideValidator serverSideValidator { get; set; } = default!;
        private GoogleCaptcha captcha;
        private bool submitted;

        private async Task SubmitForm()
        {
            if(!captcha.IsValid)
            {
                serverSideValidator.DisplayErrors(new Dictionary<string, List<string>>
                {
                    { "Captcha", new List<string>{ "You must complete the captcha" } }
                });
                notification.ShowError("You must complete the captcha");
                return;
            }

            var response = await _formService.SubmitContactForm();
            if (response?.Errors?.Any() ?? false)
            {
                serverSideValidator.DisplayErrors(response.Errors);
            }

            notification.Show(response);
            submitted = response.State == ResultState.Success;
        }
    }
}
