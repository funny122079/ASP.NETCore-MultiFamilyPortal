using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmRolesTab
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<CRMContactRole> _roles = new();

        protected override async Task OnInitializedAsync()
        {
            await UpdateRoles();
        }

        private async Task UpdateRoles()
        {
            var roles = await _client.GetFromJsonAsync<IEnumerable<CRMContactRole>>("/api/admin/contacts/crm-roles");
            _roles.ReplaceRange(roles);
        }

        private async Task OnCreate(GridCommandEventArgs args)
        {
            var item = args.Item as CRMContactRole;
            await _client.PostAsJsonAsync("/api/admin/contacts/crm-role/create", item);
            await UpdateRoles();
        }

        private async Task OnUpdate(GridCommandEventArgs args)
        {
            var item = args.Item as CRMContactRole;
            await _client.PutAsJsonAsync($"/api/admin/contacts/crm-role/update/{item.Id}", item);
            await UpdateRoles();
        }

        private async Task OnDelete(GridCommandEventArgs args)
        {
            var item = args.Item as CRMContactRole;
            if (item.SystemDefined)
                return;

            await _client.DeleteAsync($"/api/admin/contacts/crm-role/delete/{item.Id}");
            await UpdateRoles();
        }
    }
}
