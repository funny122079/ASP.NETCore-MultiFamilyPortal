using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class TimelessInfo
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string UpperTitle { get; set; }

        [Parameter]
        public int UpperNumber { get; set; }

        [Parameter]
        public string MiddleTitle { get; set; }

        [Parameter]
        public int MiddleNumber { get; set; }

        [Parameter]
        public string LowerTitle { get; set; }

        [Parameter]
        public int LowerNumber { get; set; }

    }
}