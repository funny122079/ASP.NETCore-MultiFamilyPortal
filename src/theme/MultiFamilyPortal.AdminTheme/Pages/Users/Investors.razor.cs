using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class Investors
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<UserAccountResponse> _data = new ObservableRangeCollection<UserAccountResponse>();

        protected override async Task OnInitializedAsync()
        {
            _data.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<UserAccountResponse>>("/api/admin/contacts/investors"));
        }
    }
}
