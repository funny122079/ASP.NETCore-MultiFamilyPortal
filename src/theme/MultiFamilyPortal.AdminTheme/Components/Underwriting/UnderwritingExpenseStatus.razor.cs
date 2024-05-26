using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingExpenseStatus
    {
        [Parameter]
        public UnderwritingGuidance Guidance { get; set; }
        
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}