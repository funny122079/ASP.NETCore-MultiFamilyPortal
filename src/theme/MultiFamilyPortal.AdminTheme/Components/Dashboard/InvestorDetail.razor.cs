using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class InvestorDetail
    {
        [Parameter]
        public DashboardInvestor Investor { get; set; }

        [Parameter]
        public EventCallback OnInvestorUpdated { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<InvestorDetail> _logger { get; set; }

        [Inject]
        private ITimeZoneService _timezoneService { get; set; }

        private string _localTime;
        private TimezoneData  _timezone;

        protected override void OnParametersSet()
        {
            if (Investor == null)
                return;

            var investorTimeZone = HandleTimeZone(Investor.Timezone);
            _timezone = GetIntialsTimezone(investorTimeZone);

            if (string.IsNullOrEmpty(investorTimeZone))
                _localTime = "Unknown";
            else
                try
                {
                    _localTime = _timezoneService.GetLocalTimeByTimeZone(investorTimeZone).ToShortTimeString();
                }
                catch
                {
                    _localTime = "Unknown";
                }
        }

        private async Task UpdateInvestor()
        {
            Investor.Contacted = true;
            
            try
            {
                await _client.PutAsJsonAsync<DashboardInvestor>($"/api/admin/dashboard/investors/{Investor.Id}", Investor);
                await OnInvestorUpdated.InvokeAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Put investor request unsuccessful " + DateTimeOffset.UtcNow);
            }
        }

        private string HandleTimeZone(string userInput)
        {
            userInput = userInput.Trim();

            if (string.IsNullOrEmpty(userInput))
                return userInput;

            if (userInput.Length >= 7)
            {
                return _timezoneService.Timezones.FirstOrDefault(x => x.Name.ToLower().Contains(userInput.ToLower()))?.Name;
            }

            var timezone = _timezoneService.Timezones.FirstOrDefault(x => x.Intials == userInput);

            switch (timezone)
            {
                case null when !userInput.Contains("UTC") && !userInput.Contains("GMT"):
                    return "";
                case null:
                    {
                        userInput = userInput.Replace(" ", "");

                        if (userInput.Contains("GMT"))
                        {
                            userInput.Replace("GMT", "UTC");
                        }

                        return _timezoneService.Timezones.FirstOrDefault(x => userInput.Contains(x.Intials))?.Name;
                    }
                default:
                    return timezone.Name;
            }
        }

        private TimezoneData GetIntialsTimezone(string timezone)
        {
           return _timezoneService.Timezones.FirstOrDefault(x => x.Name== timezone);
        }
    }
}
