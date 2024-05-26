using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class Subscribers
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<Subscriber> _subscribers = new();
        protected override async Task OnInitializedAsync()
        {
            _subscribers.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<Subscriber>>("/api/admin/users/subscribers"));
        }
    }
}
