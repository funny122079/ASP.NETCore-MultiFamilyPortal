using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class InvestorList
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public IEnumerable<DashboardInvestor> Investors { get; set; }

        [Parameter]
        public EventCallback OnInvestorUpdated { get; set; }

        private DashboardInvestor _selectedInvestor;

        private void ShowContact(DashboardInvestor investor) => _selectedInvestor = investor;

        private async Task RefreshContactsAsync()
        {
            await OnInvestorUpdated.InvokeAsync();
            _selectedInvestor = null;
        }
    }
}