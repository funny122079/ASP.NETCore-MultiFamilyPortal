using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Views;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Components
{
    public partial class HomeRecentPostWidget
    {
        [Parameter]
        public PostSummaryView Post { get; set; } = default !;
    }
}
