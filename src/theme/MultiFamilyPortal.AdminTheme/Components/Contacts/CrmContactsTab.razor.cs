using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactsTab
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private ProtectedSessionStorage _protectedSessionStorage { get; set; }

        private const string ShowListSessionStorage = "ShowContactList";
        private IEnumerable<CRMContact> _allContacts;
        private readonly ObservableRangeCollection<CRMContact> _contacts = new();
        private readonly ObservableRangeCollection<CRMContactRole> _roles = new();
        private readonly ObservableRangeCollection<string> _markets = new ();
        private bool _showList = true;
        private CRMContact _newContact = null;
        private string _query;
        private string _selectedRole;
        private string _selectedMarket;

        protected override async Task OnInitializedAsync()
        {
            _selectedMarket = "All";
            _markets.Add(_selectedMarket);
            _markets.AddRange(await _client.GetFromJsonAsync<IEnumerable<string>>("/api/admin/underwriting/markets"));
            var roles = await _client.GetFromJsonAsync<IEnumerable<CRMContactRole>>("/api/admin/contacts/crm-roles");
            _roles.Clear();
            _roles.Add(new CRMContactRole
            {
                Name = "All"
            });
            _roles.AddRange(roles);
            _selectedRole = _roles.First().Name;
            await UpdateContacts();
            await GetTabAsync();
        }

        private async Task UpdateContacts()
        {
            _allContacts = await _client.GetFromJsonAsync<IEnumerable<CRMContact>>("/api/admin/contacts/crm-contacts");
            FilterResults();
        }

        private void AddNewContact()
        {
            _newContact = new CRMContact
            {
                Addresses = new ObservableCollection<CRMContactAddress>(),
                Emails = new ObservableCollection<CRMContactEmail>(),
                Phones = new ObservableCollection<CRMContactPhone>(),
                Logs = new ObservableRangeCollection<CRMContactLog>(),
                Markets = new ObservableRangeCollection<CRMContactMarket>(),
                Roles = new ObservableCollection<CRMContactRole>(),
            };
        }

        private async Task OnNewContactSaved(CRMContact contact)
        {
            _newContact = contact;
            await UpdateContacts();
        }

        private void ShowContactDetails(CRMContact contact)
        {
            _navigationManager.NavigateTo($"/admin/contacts/detail/{contact.Id}");
        }

        private void OnQueryChanged(string query)
        {
            _query = query;
            FilterResults();
        }

        private void FilterResults()
        {
            var filtered = _allContacts;
            if(!string.IsNullOrEmpty(_query))
                filtered = filtered.Where(x =>
                    x.FirstName.Contains(_query, StringComparison.InvariantCultureIgnoreCase) ||
                    x.LastName.Contains(_query, StringComparison.InvariantCultureIgnoreCase) ||
                    x.Company.Contains(_query, StringComparison.InvariantCultureIgnoreCase) ||
                    x.Emails.Any(e => e.Email.Contains(_query, StringComparison.InvariantCultureIgnoreCase)));

            if (_roles.First(x => x.Name == _selectedRole).Id != default)
                filtered = filtered.Where(x => x.Roles.Any(r => r.Name == _selectedRole));

            if (!string.IsNullOrEmpty(_selectedMarket) && _markets.IndexOf(_selectedMarket) > 0)
                filtered = filtered.Where(x => x.Markets.Any(m => m.Name == _selectedMarket));

            _contacts.ReplaceRange(filtered.OrderBy(x => x.LastName));
        }

        private async Task HandleViewAsync(bool isList)
        {
            _showList = isList;
            await SetTabAsync();
        }

        private async Task SetTabAsync() => await _protectedSessionStorage.SetAsync(ShowListSessionStorage, _showList);

        private async Task GetTabAsync()
        {
            var isCard = await _protectedSessionStorage.GetAsync<bool>(ShowListSessionStorage);
            if(isCard.Success)
               _showList = isCard.Value;
        }
    }
}
