using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Dtos.Portfolio;

namespace MultiFamilyPortal.AdminTheme.Pages.Properties.AssetsUnderManagement
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class Index
    {
        [Inject]
        private HttpClient _client { get; set; }

        private IEnumerable<PortfolioSummary> _portfolio;

        protected override async Task OnInitializedAsync()
        {
            _portfolio = await _client.GetFromJsonAsync<IEnumerable<PortfolioSummary>>("/api/portfolio");
        }
    }
}
