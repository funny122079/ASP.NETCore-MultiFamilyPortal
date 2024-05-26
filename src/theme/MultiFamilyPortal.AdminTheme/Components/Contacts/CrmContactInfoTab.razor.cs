using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactInfoTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private IEnumerable<CRMContactRole> _allRoles;
        private List<Guid> _roles = new();

        protected override async Task OnInitializedAsync()
        {
            if (Contact.Roles?.Any() ?? false)
                _roles = new List<Guid>(Contact.Roles.Select(x => x.Id));

            _allRoles = await _client.GetFromJsonAsync<IEnumerable<CRMContactRole>>("/api/admin/contacts/crm-roles");
        }

        private void OnRolesUpdated(List<Guid> roles)
        {
            _roles = roles;
            Contact.Roles = _allRoles.Where(x => roles.Contains(x.Id)).ToList();
        }
    }
}
