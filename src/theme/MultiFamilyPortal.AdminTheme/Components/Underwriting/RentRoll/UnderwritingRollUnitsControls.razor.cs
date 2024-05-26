using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingRollUnitsControls
    {
        [Parameter]
        public DisplayUnit Unit { get; set; }

        [Parameter]
        public EventCallback<DisplayUnit> UnitChanged { get; set; }

        [Parameter]
        public EventCallback OnAddFloorUnits { get; set; }

        private bool _confirmation = false;
        private bool _canSave => !string.IsNullOrEmpty(Unit?.UnitName);

        private async Task UpdateUnit()
        {
            if (!_canSave)
                return;

            if(Unit.Id == default)
            {
                Unit.Add();
            }
            else
            {
                Unit.Update();
            }

            await UnitChanged.InvokeAsync(null);
        }

        private async Task RemoveUnit()
        {
            Unit.Remove();
            await UnitChanged.InvokeAsync(null);
        }

        private async Task AddAnotherAsync()
        {
            if (!_canSave)
                return;

            Unit.Add();
            Unit = new DisplayUnit(Unit.FloorPlan);
            await OnAddFloorUnits.InvokeAsync();
        }
    }
}
