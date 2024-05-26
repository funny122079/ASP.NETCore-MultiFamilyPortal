using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting.Reports;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Reports
{
    public partial class InvestmentTierEditor
    {
        [Required]
        [Parameter]
        public Guid PropertyId { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public List<UnderwritingInvestmentTier> Tiers { get; set; }

        [Parameter]
        public EventCallback OnUpdated { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        public bool _isNew { get; set; }

        protected override void OnInitialized()
        {
            _isNew = string.IsNullOrEmpty(Name);

            if(Tiers is null)
            {
                Tiers = new List<UnderwritingInvestmentTier>
                {
                    new UnderwritingInvestmentTier
                    {
                        Investment = 100000,
                        RoROnSale = 0,
                        PreferredRoR = 0.08
                    }
                };
            }
        }

        private async Task OnSave()
        {
            await _client.PostAsJsonAsync($"/api/admin/reports/investment-tiers/{PropertyId}/{Name}", Tiers);
            await OnUpdated.InvokeAsync();
        }

        private async Task OnDelete()
        {
            await _client.DeleteAsync($"/api/admin/reports/investment-tiers/{PropertyId}/{Name}");
            await OnUpdated.InvokeAsync();
        }
    }
}
