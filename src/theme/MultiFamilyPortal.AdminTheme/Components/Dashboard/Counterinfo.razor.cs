using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class CounterInfo
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string TimeFrame { get; set; }

        [Parameter]
        public string TimeFrameColor { get; set; }

        [Parameter]
        public  DashboardUnderwritingResponse Underwriting { get; set; }
    }
}