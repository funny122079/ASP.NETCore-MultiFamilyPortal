using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI.Extensions;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages.Subscriber
{
    public partial class Unsubscribe
    {
        [Parameter]
        public string email { get; set; }

        [Inject]
        private IBlogSubscriberService _blogSubscriberService { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private string _code;
        private bool unsubscribed;

        protected override async Task OnInitializedAsync()
        {
            if(_navigationManager.TryGetQueryString(nameof(_code), out _code))
                unsubscribed = await _blogSubscriberService.Unsubscribe(email, _code);
        }
    }
}
