using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingFloorPlansEditWindow
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public UnderwritingAnalysisModel FloorPlan { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisModel> FloorPlanChanged { get; set; }

        private string _title;

        protected override void OnInitialized()
        {
            if(FloorPlan.Id == default)
            {
                _title = "Add Floor Plan";
            }
            else
            {
                _title = "Edit Floor Plan";
            }
        }

        private async Task CloseAsync()
        {
            await FloorPlanChanged.InvokeAsync(null);
        }
    }
}
