using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages.Subscriber
{
    public partial class Confirmation
    {
        [Parameter]
        public Guid confirmationCode { get; set; }

        [Inject]
        private IBlogSubscriberService _subscriberService { get; set; }

        private string email;

        protected override async Task OnInitializedAsync()
        {
            email = await _subscriberService.SubscriberConfirmation(confirmationCode);
        }
    }
}
