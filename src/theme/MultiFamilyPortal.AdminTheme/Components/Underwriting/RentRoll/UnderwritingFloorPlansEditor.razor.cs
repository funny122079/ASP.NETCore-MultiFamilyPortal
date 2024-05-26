using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingFloorPlansEditor
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public UnderwritingAnalysisModel FloorPlan { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisModel> FloorPlanChanged { get; set; }

        private async Task AddFloorAsync()
        {
            if (string.IsNullOrEmpty(FloorPlan.Name))
                return;

            var model = new UnderwritingAnalysisModel
            {
                Area = FloorPlan.Area,
                Baths = FloorPlan.Baths,
                Beds = FloorPlan.Beds,
                CurrentRent = FloorPlan.CurrentRent,
                MarketRent = FloorPlan.MarketRent,
                Name = FloorPlan.Name?.TrimEnd(),
                TotalUnits = FloorPlan.TotalUnits,
                Units = new List<UnderwritingAnalysisUnit>(),
                Upgraded = FloorPlan.Upgraded,
            };

            if (!Property.Models.Any(x => string.Equals(x.Name, model.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                Property.AddModel(model);
            }

            await CloseAsync();
        }

        private async Task EditFloorAsync()
        {
            if (string.IsNullOrEmpty(FloorPlan.Name))
                return;

            var model = Property.Models.FirstOrDefault(x => x.Id == FloorPlan.Id);

            model.Area = FloorPlan.Area;
            model.Baths = FloorPlan.Baths;
            model.Beds = FloorPlan.Beds;
            model.CurrentRent = FloorPlan.CurrentRent;
            model.MarketRent = FloorPlan.MarketRent;
            model.Name = FloorPlan.Name;
            model.TotalUnits = FloorPlan.TotalUnits;
            model.Units = FloorPlan.Units;
            model.Upgraded = FloorPlan.Upgraded;

            await CloseAsync();
        }

        private async Task DeleteFloorAsync()
        {
            Property.RemoveModel(FloorPlan);
            await CloseAsync();
        }

        private Task CloseAsync() => FloorPlanChanged.InvokeAsync(null);
    }
}
