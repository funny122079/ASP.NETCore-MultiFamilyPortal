using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MultiFamilyPortal.Configuration;
using MultiFamilyPortal.CoreUI.Models;

namespace MultiFamilyPortal.CoreUI
{
    public partial class GoogleCaptcha
    {
        private readonly string Id = $"google_recaptcha_{Guid.NewGuid()}";

        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        [Inject]
        private GoogleCaptchaOptions _options { get; set; }

        public bool IsValid { get; private set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !string.IsNullOrEmpty(_options.SiteKey))
            {
                await _jsRuntime.InvokeAsync<int>("MFPortal.GoogleCaptcha", DotNetObjectReference.Create(this), Id, _options.SiteKey);
            }
            else if(string.IsNullOrEmpty(_options.SiteKey))
            {
                IsValid = true;
            }
        }

        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public async void CallbackOnSuccess(string response)
        {
            using var client = new HttpClient();
            var request = new CaptchaRequest
            {
                Secret = _options.SecretKey,
                Response = response
            };
            using var httpResponseMessage = await client.PostAsJsonAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_options.SecretKey}&response={response}", request);
            if(httpResponseMessage.IsSuccessStatusCode)
            {
                var result = await httpResponseMessage.Content.ReadFromJsonAsync<CaptchaResponse>();
                IsValid = result.Success;
            }
        }

        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnExpired(string response)
        {
            IsValid = false;
        }
    }
}
