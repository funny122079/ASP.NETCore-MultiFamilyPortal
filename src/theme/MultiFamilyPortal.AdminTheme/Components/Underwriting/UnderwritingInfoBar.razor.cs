using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingInfoBar
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public ColorCode Color { get; set; }
    }
}
