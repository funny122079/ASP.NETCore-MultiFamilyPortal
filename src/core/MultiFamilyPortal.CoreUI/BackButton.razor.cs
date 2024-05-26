using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class BackButton
    {
        [Parameter]
        public EventCallback OnNavigateBack { get; set; }
    }
}
