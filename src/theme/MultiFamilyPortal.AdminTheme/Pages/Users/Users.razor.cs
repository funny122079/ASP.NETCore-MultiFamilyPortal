using System.Net.Http.Json;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    public partial class Users : IDisposable
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly IEnumerable<SelectableRole> _roles = new[]
            {
                new SelectableRole { Display = PortalRoles.BlogAuthor.Humanize(LetterCasing.Title), Role = PortalRoles.BlogAuthor },
                new SelectableRole { Display = PortalRoles.PortalAdministrator.Humanize(LetterCasing.Title), Role = PortalRoles.PortalAdministrator },
                new SelectableRole { Display = PortalRoles.Underwriter, Role = PortalRoles.Underwriter },
                new SelectableRole { Display = PortalRoles.Mentor, Role = PortalRoles.Mentor },
            };

        private readonly FilterableUsers _internalUsers = new FilterableUsers();
        private readonly FilterableUsers _externalUsers = new FilterableUsers();
        private bool _disposedValue;

        protected override async Task OnInitializedAsync()
        {
            await RefreshUsers();
        }

        private async Task RefreshUsers()
        {
            var users = await _client.GetFromJsonAsync<IEnumerable<UserAccountResponse>>("/api/admin/users");
            var roles = _roles.Select(x => x.Role);
            var internalUsers = users.Where(x => x.Roles.Any(role => roles.Contains(role)));
            _internalUsers.UpdateUsers(internalUsers);
            var externalUsers = users.Where(x => !internalUsers.Any(u => x.Id == u.Id));
            _externalUsers.UpdateUsers(externalUsers);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _internalUsers.Dispose();
                    _externalUsers.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
