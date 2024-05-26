using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CRMContactLogEditor
    {
        [Parameter]
        public CRMContactLog Value { get; set; }

        [Parameter]
        public EventCallback<CRMContactLog> OnSave { get; set; }

        private async Task OnClose()
        {
            await OnSave.InvokeAsync(null);
        }

        private async Task OnSaveExecuted()
        {
            await OnSave.InvokeAsync(Value);
        }
    }
}
