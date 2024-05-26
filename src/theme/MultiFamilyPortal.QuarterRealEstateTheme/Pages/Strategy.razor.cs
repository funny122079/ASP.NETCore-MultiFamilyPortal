using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages
{
    public partial class Strategy
    {
        [Inject]
        private HttpClient _client { get; set; }

        private CustomContent _content;

        protected override async Task OnInitializedAsync()
        {
            _content = await _client.GetFromJsonAsync<CustomContent>($"/api/content/{PortalPage.Strategy}");
        }
    }
}
