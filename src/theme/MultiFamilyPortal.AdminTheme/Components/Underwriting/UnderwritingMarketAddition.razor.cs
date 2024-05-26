using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMarketAddition
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<UnderwritingMarketAddition> _logger { get; set; }

        [Parameter]
        public IEnumerable<UnderwritingGuidance> Guidance { get; set; }

        [Parameter]
        public bool Editable { get; set; }

        [Parameter]
        public EventCallback UpdateGuidance { get; set; }
        private readonly ObservableRangeCollection<string> _markets = new();
        private PortalNotification notification;
        private readonly IEnumerable<CostType> _expenseTypes = Enum.GetValues<CostType>();
        private string _newMarket;
        private  bool _conformation = false;
        protected override async Task OnInitializedAsync()
        {
            await LoadMarkets();
            _newMarket = Guidance.FirstOrDefault().Market;
            _newMarket = string.IsNullOrEmpty(_newMarket) && Editable ? "Default" : _newMarket;
            _conformation = false;
        }

        private async Task LoadMarkets()
        {
            try
            {
                _markets.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<string>>("/api/admin/underwriting/markets"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acquring markets for autocomplete.");
            }
        }

        private async Task ProcessMarket()
        {
            if (string.IsNullOrWhiteSpace(_newMarket))
            {
                notification.ShowError("Market name is required.");
                return;
            }

            if (Editable)
                await OnEditMarket();
            else
                await OnAddMarket();
        }

        private async Task OnAddMarket()
        {
            try
            {
                foreach (var market in Guidance)
                {
                    market.Market = _newMarket;
                    market.Id = Guid.Empty;
                    await _client.PostAsJsonAsync($"/api/admin/underwriting/guidance", market);
                }

                notification.ShowSuccess("Markets successfully added.");
                _logger.LogInformation($"Markets added to guidance");
                await UpdateGuidance.InvokeAsync();
            }
            catch (Exception ex)
            {
                notification.ShowError("An unknown error occurred while adding new market");
                _logger.LogError(ex, "Error creating a new market");
            }
        }

        private async Task OnEditMarket()
        {   
            try
            {
                foreach (var market in Guidance)
                {
                    await _client.PutAsJsonAsync($"/api/admin/underwriting/guidance/{market.Id}", market);
                }

                notification.ShowSuccess("Markets successfully updated.");
                _logger.LogInformation($"Markets added to guidance");
                await UpdateGuidance.InvokeAsync();
            }
            catch (Exception ex)
            {
                notification.ShowError("An unknown error occurred while updating the market");
                _logger.LogError(ex, "Error updating markets");
            }
        }

        private async Task OnRemoveMarket()
        {
            try
            {
                foreach (var market in Guidance)
                {
                    await _client.DeleteAsync($"/api/admin/underwriting/guidance/{market.Id}");
                }
                
                var message  = "Markets successfully deleted.";
                notification.ShowSuccess(message);
                _logger.LogInformation(message);
                await UpdateGuidance.InvokeAsync();
            }
            catch (Exception ex)
            {
                notification.ShowError("An unknown error occurred while removing the market");
                _logger.LogError(ex, "Error deleting markets");
            }
        }
    }
}