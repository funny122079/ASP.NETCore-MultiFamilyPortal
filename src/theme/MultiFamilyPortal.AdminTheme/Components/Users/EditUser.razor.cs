using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.CoreUI;

namespace MultiFamilyPortal.AdminTheme.Components.Users
{
    public partial class EditUser
    {
        [Parameter]
        public CreateUserRequest User { get; set; }

        [Parameter]
        public EventCallback<CreateUserRequest> UserChanged { get; set; }

        [Parameter]
        public string UserId { get; set; }

        [Parameter]
        public IEnumerable<SelectableRole> Roles { get; set; }

        [Inject]
        private ILogger<EditUser> _logger { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private PortalNotification notification { get; set; }

        private async Task UpdateUser()
        {
            try
            {
                _logger.LogInformation("Updating user");
                using var response = await _client.PutAsJsonAsync($"/api/admin/users/{UserId}", User);
                _logger.LogInformation($"User update : {response.StatusCode}");
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("The user was updated successfully");
                        await OnInitializedAsync();
                        break;
                    case HttpStatusCode.NotFound:
                        notification.ShowWarning("The user was not found");
                        break;
                    default:
                        notification.ShowError("An unknown error occurred while updating the user");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"User update failed : {ex.Message}");
            }

            await UserChanged.InvokeAsync(null);
        }
    }
}
