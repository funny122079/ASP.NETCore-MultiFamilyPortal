using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactCardList
    {
        [Parameter]
        public IEnumerable<CRMContact> Contacts { get; set; }

        [Parameter]
        public EventCallback AddContact { get; set; }

        [Parameter]
        public EventCallback<CRMContact> ShowContactDetails { get; set; }

        public int _pageSize { get; set; } = 15;
        public int _page { get; set; } = 1; 
    }
}