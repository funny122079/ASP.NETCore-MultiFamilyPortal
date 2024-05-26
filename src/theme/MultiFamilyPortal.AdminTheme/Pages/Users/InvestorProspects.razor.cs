using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class InvestorProspects
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<InvestorProspect> _data = new ();
        protected override async Task OnInitializedAsync()
        {
            _data.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<InvestorProspect>>("/api/admin/contacts/investors/prospects"));
        }
    }
}
