using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactMarketsTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private IEnumerable<string> _markets;
        private int _index = -1;

        protected override async Task OnInitializedAsync()
        {
            _markets = await _client.GetFromJsonAsync<IEnumerable<string>>("/api/admin/underwriting/markets");
        }

        private void OnMarketAdded(GridCommandEventArgs args)
        {
            Contact.Markets.Add(args.Item as CRMContactMarket);
        }

        private void OnMarketEditing(GridCommandEventArgs args)
        {
            _index = Contact.Markets.IndexOf(args.Item);
        }

        private void OnMarketUpdated(GridCommandEventArgs args)
        {
            var updated = args.Item as CRMContactMarket;
            var existing = Contact.Markets.ElementAt(_index);
            existing.Name = updated.Name;
            _index = -1;
        }

        private void OnMarketDeleted(GridCommandEventArgs args)
        {
            var index = Contact.Markets.IndexOf(args.Item);
            var item = Contact.Markets.ElementAt(index);
            Contact.Markets.Remove(item);
        }
    }
}
