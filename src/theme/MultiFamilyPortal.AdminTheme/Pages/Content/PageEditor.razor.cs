using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Content
{
    public partial class PageEditor
    {
        [Parameter]
        public string id { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private CustomContent _content;
        private PortalNotification _notification;
        private bool notFound;
        private string errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var response = await _client.GetAsync($"/api/admin/content/{id}");

                if(response.IsSuccessStatusCode)
                    _content = await response.Content.ReadFromJsonAsync<CustomContent>();
                else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    notFound = true;
                else
                    throw new Exception($"Unable to load content: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                // TODO: Add Logging
                errorMessage = ex.Message;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if(firstRender && !string.IsNullOrEmpty(errorMessage))
            {
                _notification.ShowError(errorMessage);
            }
        }

        private async Task OnUpdate()
        {
            using var response = await _client.PostAsJsonAsync($"/api/admin/content/update/{_content.Id}", _content);
            if (response.IsSuccessStatusCode)
                _notification.ShowSuccess("Page updated successfully");
            else
                _notification.ShowWarning("Unable to update page");
        }

        private void OnNavigateBack()
        {
            _navigationManager.NavigateTo("/admin/pages");
        }
    }
}
