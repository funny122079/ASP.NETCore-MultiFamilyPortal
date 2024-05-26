using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.CoreUI;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Users
{
    public partial class UserList
    {
        [Parameter]
        public bool AllowEditing { get; set; }

        [Parameter]
        public FilterableUsers Model { get; set; }

        [Parameter]
        public IEnumerable<SelectableRole> Roles { get; set; }

        [Parameter]
        public EventCallback OnUpdated { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<UserList> Logger { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal User { get; set; }

        private CreateUserRequest _createUser;
        private CreateUserRequest _editUser;
        private string _editId;

        private PortalNotification notification { get; set; }

        private void OnAddUser()
        {
            _createUser = new CreateUserRequest();
        }

        private async Task OnUserUpdated()
        {
            _createUser = null;
            _editUser = null;
            _editId = null;
            await OnUpdated.InvokeAsync();
        }

        private async Task DeleteUser(GridCommandEventArgs args)
        {
            var user = args.Item as UserAccountResponse;

            try
            {
                Logger.LogInformation($"Deleting user account {user.Email}");
                using var response = await _client.DeleteAsync($"/api/admin/users/{user.Id}");
                Logger.LogInformation($"User deletion : {response.StatusCode}");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("The user was deleted successfully");
                        await OnUpdated.InvokeAsync();
                        break;
                    case HttpStatusCode.NotFound:
                        notification.ShowWarning("The user was not found");
                        break;
                    default:
                        notification.ShowError("An unknown error occurred while deleting the user");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"User deletion failed : {ex.Message}");
            }
        }

        private void OnEditUser(GridCommandEventArgs args)
        {
            var user = args.Item as UserAccountResponse;
            _editId = user.Id;

            _editUser = new CreateUserRequest
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                UseLocalAccount = user.LocalAccount,
                Roles = user.Roles.ToList()
            };
        }
    }
}
