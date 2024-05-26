using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmCreateContact
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        [Parameter]
        public EventCallback<CRMContact> ContactChanged { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private PortalNotification _notification;

        private async Task OnNavigateBack()
        {
            Contact = null;
            await ContactChanged.InvokeAsync(null);
        }

        private async Task SaveContact()
        {
            using var response = await _client.PostAsJsonAsync("/api/admin/contacts/crm-contact/create", Contact);

            if (response.IsSuccessStatusCode)
            {
                _notification.ShowSuccess("Contact successfully created");
                await ContactChanged.InvokeAsync(null);
            }
            else
            {
                _notification.ShowWarning("Unable to create contact");
            }
        }
    }
}
