using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingRentRollAddition
    {
        [Parameter]
        public DisplayUnit Unit { get; set; }

        [Parameter]
        public EventCallback<DisplayUnit> UnitChanged { get; set; }

        private async Task OnNavigateBack() => await UnitChanged.InvokeAsync(null);
    }
}
