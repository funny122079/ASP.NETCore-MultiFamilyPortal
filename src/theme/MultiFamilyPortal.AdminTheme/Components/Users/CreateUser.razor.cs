using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.CoreUI;

namespace MultiFamilyPortal.AdminTheme.Components.Users
{
    public partial class CreateUser
    {
        [Parameter]
        public CreateUserRequest User { get; set; }

        [Parameter]
        public EventCallback<CreateUserRequest> UserChanged { get; set; }

        [Parameter]
        public IEnumerable<SelectableRole> Roles { get; set; }

        [Inject]
        private ILogger<CreateUser> _logger { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private PortalNotification notification { get; set; }

        private bool CanCreateNewUser()
        {
            if (User is null || string.IsNullOrEmpty(User.Email) || string.IsNullOrEmpty(User.FirstName) || string.IsNullOrEmpty(User.LastName) || !(User.Roles?.Any() ?? false))
                return false;
            return true;
        }

        private async Task OnCreateNewUser()
        {
            try
            {
                _logger.LogInformation("Creating new user account");
                using var response = await _client.PostAsJsonAsync("/api/admin/users/create", User);
                _logger.LogInformation($"User creation : {response.StatusCode}");
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.NoContent:
                        notification.ShowError("Failed to create user");
                        break;
                    case HttpStatusCode.OK:
                        notification.ShowSuccess("The user was created successfully");
                        break;
                    default:
                        notification.ShowError("An unknown error occurred while created the user");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"User password creation failed : {ex.Message}");
            }

            User = null;
            await UserChanged.InvokeAsync(null);
        }
    }
}
