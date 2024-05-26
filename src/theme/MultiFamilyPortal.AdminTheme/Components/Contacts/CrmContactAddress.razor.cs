using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactAddress
    {        
        [Parameter]
        public CRMContactAddress Address { get; set; }

        [Parameter]
        public EventCallback<CRMContactAddress> AddressChanged {get; set;}

        private async Task UpdateParent() => await AddressChanged.InvokeAsync(Address);
    }
}