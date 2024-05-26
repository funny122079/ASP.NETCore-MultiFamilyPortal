using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Wizard
{
    public partial class FinancialsWizardStep
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public EventCallback ExpensesUpdated { get; set; }

        private async Task OnExpensesUpdated()
        {
            await ExpensesUpdated.InvokeAsync();
        }
    }
}
