using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactCard
    {
        [Parameter]
        public CRMContact Contact { get; set; }
    }
}
