using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PortalLocalizedTime
    {
        private const string DefaultFormat = "MMMM dd yyyy hh:mm tt";
        [Parameter]
        public DateTimeOffset Date { get; set; }

        [Parameter]
        public string Format { get; set; } = DefaultFormat;

        [Parameter]
        public string Class { get; set; }

        [Inject]
        private ITimeZoneService _timeZoneService { get; set; }

        private string _formattedString;

        protected override void OnInitialized()
        {
            _formattedString = Date.ToString(Format);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var local = await _timeZoneService.GetLocalDateTime(Date);
            _formattedString = local.ToString(Format);
            await InvokeAsync(StateHasChanged);
        }
    }
}
