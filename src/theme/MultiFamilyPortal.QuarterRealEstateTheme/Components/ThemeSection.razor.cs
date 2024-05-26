using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Components
{
    public partial class ThemeSection
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
