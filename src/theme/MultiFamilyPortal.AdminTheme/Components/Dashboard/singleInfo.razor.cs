using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class SingleInfo
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string TimeFrame { get; set; }

        [Parameter]
        public string TimeFrameColor { get; set; }

        [Parameter]
        public string Number { get; set; }

        [Parameter]
        public string IndicatorTitle { get; set; }

        [Parameter]
        public string IndicatorPercentage { get; set; }

        [Parameter]
        public bool IsPositiveChange { get; set; }
    }
}