using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MultiFamilyPortal.CoreUI
{
    public partial class OAuthLoginButton
    {
        [Parameter]
        public AuthenticationScheme Scheme { get; set; }

        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private string _text;
        private ElementReference form;

        protected override void OnParametersSet()
        {
            if (Scheme is null)
                return;

            if (Scheme.Name == "Google")
                _text = "Sign in with Google";
            else if (Scheme.Name == "Microsoft")
                _text = "Personal, Company or School";
        }

        private async Task Clicked()
        {
            if (Scheme is null)
                return;

            await _jsRuntime.InvokeVoidAsync("MFPortal.SubmitForm", form);
        }
    }
}
