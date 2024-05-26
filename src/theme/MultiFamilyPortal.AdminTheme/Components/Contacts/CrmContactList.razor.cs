using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactList
    {
        [Parameter]
        public IEnumerable<CRMContact> Contacts { get; set; }

        [Parameter]
        public EventCallback AddContact { get; set; }

        [Parameter]
        public EventCallback<CRMContact> ShowContactDetails { get; set; }

        private async Task OnRowClicked(GridRowClickEventArgs args)
        {
            var contact = args.Item as CRMContact;
            if(contact != null)
                await ShowContactDetails.InvokeAsync(contact);
        }
    }
}
