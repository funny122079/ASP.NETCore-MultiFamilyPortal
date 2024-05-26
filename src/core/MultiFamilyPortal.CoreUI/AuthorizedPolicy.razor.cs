using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class AuthorizedPolicy
    {
        [Parameter]
        public string Policy { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
