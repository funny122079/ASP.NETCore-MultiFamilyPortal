using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Financials
{
    public partial class UnderwritingSellerInfoTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
        }
    }
}
