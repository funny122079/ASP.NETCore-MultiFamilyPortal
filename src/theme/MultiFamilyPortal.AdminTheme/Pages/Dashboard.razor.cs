using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.CoreUI;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Pages
{
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    public partial class Dashboard
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<Dashboard> _logger { get; set; }

        private DashboardInvestorsResponse _investors = new();
        private DashboardBlogResponse _blog = new();
        private DashboardActivityResponse _activity = new();
        private DashboardUnderwritingResponse _underwriting = new();
        private PortalNotification notification { get; set; }
        private string _message = "Error fetching dashboard data";

        protected override async Task OnInitializedAsync()
        {
            await GetUnderWritingAsync();
            await GetActivityAsync();
            await GetBlogSubsAsync();
            await GetInvestorsAsync();
        }

        private async Task GetUnderWritingAsync()
        {
            try
            {
                _underwriting = await _client.GetFromJsonAsync<DashboardUnderwritingResponse>("/api/admin/dashboard/underwriting");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, _message + "\t underwriting : " + DateTimeOffset.UtcNow);
                notification.ShowError(_message);
            }
        }

        private async Task GetInvestorsAsync()
        {
            try
            {
                _investors = await _client.GetFromJsonAsync<DashboardInvestorsResponse>("/api/admin/dashboard/investors");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, _message + "\t investors : " + DateTimeOffset.UtcNow);
                notification.ShowError(_message);
            }
        }

        private async Task GetActivityAsync()
        {
            try
            {
                _activity = await _client.GetFromJsonAsync<DashboardActivityResponse>("/api/admin/dashboard/activity");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, _message + "\t activity : " + DateTimeOffset.UtcNow);
                notification.ShowError(_message);
            }
        }

        private async Task GetBlogSubsAsync()
        {
            try
            {
                _blog = await _client.GetFromJsonAsync<DashboardBlogResponse>("/api/admin/dashboard/blog");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, _message + "\t blog subscriptions : " + DateTimeOffset.UtcNow);
                notification.ShowError(_message);
            }
        }

        private async Task RefreshAsync() => await GetInvestorsAsync();
    }
}
