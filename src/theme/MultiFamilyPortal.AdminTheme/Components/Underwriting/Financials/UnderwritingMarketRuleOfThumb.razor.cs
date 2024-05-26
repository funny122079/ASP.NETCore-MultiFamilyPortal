using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Financials
{
    public partial class UnderwritingMarketRuleOfThumb
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public EventCallback OnPropertyChanged { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<UnderwritingGuidance> _rules = new();
        private PortalNotification _notification;
        private UnderwritingGuidance _selected;
        private bool showSelectedGuidance = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            showSelectedGuidance = false;
            if (!firstRender)
                return;

            try
            {
                var guidanceList = await _client.GetFromJsonAsync<IEnumerable<UnderwritingGuidance>>($"/api/admin/underwriting/guidance?market={Property.Market}");
                _rules.ReplaceRange(guidanceList);
            }
            catch (Exception ex)
            {
                _notification.ShowError(ex.Message);
            }
        }

        private void ShowUpdateUnderwriting(GridCommandEventArgs args)
        {
            _selected = args.Item as UnderwritingGuidance;
            showSelectedGuidance = true;
        }

        private async Task UpdateExpense(double v)
        {
            var target = Property.Ours.Where(x => x.Category == _selected.Category);
            if (!target.Any())
                return;

            target.FirstOrDefault().Amount = v;
            await OnPropertyChanged.InvokeAsync();
        }

        private bool IsCleanExpense(UnderwritingGuidance guidance)
        {
            return Property.Ours.Count(x => x.Category == guidance.Category) == 1;
        }
    }
}
