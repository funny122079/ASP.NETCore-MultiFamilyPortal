using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Reports
{
    public partial class UnderwritingInvestmentTiers
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private string _selectedGroup;
        private Dictionary<string, List<UnderwritingInvestmentTier>> _groups = new();
        private List<UnderwritingInvestmentTier> _group => !string.IsNullOrEmpty(_selectedGroup) && _groups.ContainsKey(_selectedGroup) ?
            _groups[_selectedGroup] : null;
        private string _reportLink => _addReportGroup || _editReportGroup || string.IsNullOrEmpty(_selectedGroup) ?
            null : $"/api/admin/reports/investment-tiers/{Property.Id}/{_selectedGroup}?refresh={DateTime.Now.Ticks}";
        private bool _addReportGroup;
        private bool _editReportGroup;

        protected override async Task OnInitializedAsync()
        {
            await Refresh();
            _selectedGroup = _groups.Keys.FirstOrDefault();

            _addReportGroup = string.IsNullOrEmpty(_selectedGroup);
        }

        private void OnAdd()
        {
            _addReportGroup = true;
            StateHasChanged();
        }

        private void OnEdit()
        {
            _editReportGroup = true;
            StateHasChanged();
        }

        private async Task Refresh()
        {
            _groups = await _client.GetFromJsonAsync<Dictionary<string, List<UnderwritingInvestmentTier>>>($"/api/admin/reports/investment-tiers/groups/{Property.Id}");
            _addReportGroup = false;
            _editReportGroup = false;
        }
    }
}
