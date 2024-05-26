using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Pages
{
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    public partial class UserProfile
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<UserProfile> _logger { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _siteUser => _user.IsInAnyRole(PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator, PortalRoles.Underwriter);

        private SerializableUser SiteUser;
        private UnderwriterGoal Goals;
        private ObservableRangeCollection<EditableLink> Links = new();
        private TelerikTabStrip _myTabStrip;
        private PortalNotification notification;
        private ChangePasswordRequest ChangePassword = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                SiteUser = await _client.GetFromJsonAsync<SerializableUser>("/api/admin/userprofile");

                if (SiteUser != null)
                {
                    Goals = SiteUser.Goals;
                    var providers = await _client.GetFromJsonAsync<IEnumerable<SocialProvider>>("/api/admin/userprofile/social-providers");

                    Links.ReplaceRange(providers.Select(x => new EditableLink
                    {
                        Icon = x.Icon,
                        Id = x.Id,
                        Name = x.Name,
                        Placeholder = x.Placeholder,
                        UriTemplate = x.UriTemplate,
                        Value = SiteUser.SocialLinks.FirstOrDefault(l => l.SocialProviderId == x.Id)?.Value
                    }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to get data");
            }
        }

        private async Task UpdateAsync()
        {
            await UpdateUser();
            await UpdateLinks();
            await UpdateGoals();
        }

        private async Task OnChangePassword(EditContext context)
        {
            var model = context.Model as ChangePasswordRequest;
            using var response = await _client.PostAsJsonAsync("/api/admin/userprofile/update/password", model);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Password updated");
            else
                notification.ShowWarning("Could not update password");
        }

        private async Task UpdateUser()
        {
            try
            {
                using var response = await _client.PostAsJsonAsync("/api/admin/userprofile/update/profile", SiteUser);

                if (!response.IsSuccessStatusCode)
                    notification.ShowWarning("Could not update profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user.");
            }
        }

        private async Task UpdateLinks()
        {
            try
            {
                List<SocialLink> links = Links.Select(l =>
                        new SocialLink { SocialProviderId = l.Id, Value = l.Value, UserId = Guid.Empty.ToString() }).ToList();

                using var response = await _client.PostAsJsonAsync("api/admin/UserProfile/update/links", links);

                if (!response.IsSuccessStatusCode)
                    notification.ShowWarning("Could not update profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user social links.");
            }
        }

        private async Task UpdateGoals()
        {
            try
            {
                using var response = await _client.PostAsJsonAsync("api/admin/UserProfile/update/goals", Goals);

                if (response.IsSuccessStatusCode)
                    notification.ShowSuccess("Profile updated");
                else
                    notification.ShowWarning("Could not update profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user goals.");
            }
        }

        private class EditableLink : SocialProvider
        {
            public string Value { get; set; }
        }
    }
}
