using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingUnitListEditor
    {
        [Parameter]
        public UnderwritingAnalysisModel FloorPlan { get; set; }

        private DisplayUnit _unit;
        private void ShowAddUnit()
        {
            _unit = new DisplayUnit(FloorPlan);
        }
    }
}
