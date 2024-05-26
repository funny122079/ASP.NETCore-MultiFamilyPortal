using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.PropertyInfo
{
    public partial class UnderwritingREIndicatorViewPort
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
