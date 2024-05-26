using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Contacts
{
    public partial class ContactDetails
    {
        [Parameter]
        public Guid id { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private CRMContact _contact;
        private PortalNotification _notification;

        protected override async Task OnInitializedAsync()
        {
            await UpdateContact();
        }

        private async Task UpdateContact()
        {
            _contact = await _client.GetFromJsonAsync<CRMContact>($"/api/admin/contacts/crm-contact/{id}");
        }

        private void OnNavigateBack()
        {
            _navigationManager.NavigateTo("/admin/contacts");
        }

        private async Task OnSaveChanges()
        {
            using var response = await _client.PutAsJsonAsync($"/api/admin/contacts/crm-contact/update/{id}", _contact);

            if (response.IsSuccessStatusCode)
                _notification.ShowSuccess("Contact has been successfully updated.");
            else
                _notification.ShowWarning("Unable to save changes");

            await UpdateContact();
        }
    }
}
