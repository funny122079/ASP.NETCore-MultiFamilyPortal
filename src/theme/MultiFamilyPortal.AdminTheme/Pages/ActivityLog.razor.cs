using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Services;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Pages
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    public partial class ActivityLog
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ITimeZoneService _timezone { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private DateTimeOffset _start = DateTimeOffset.Now.AddMonths(-1);
        private DateTimeOffset _end = DateTimeOffset.Now;
        private ObservableRangeCollection<ActivityResponse> _activities = new ();
        private string _profileId;
        private ObservableRangeCollection<UnderwriterResponse> _underwriters = new () { new UnderwriterResponse { DisplayName = "All" } };

        private ActivityResponse _newActivity;
        private ActivityResponse _updateActivity;
        private IEnumerable<ActivityType> _activityTypes = Enum.GetValues<ActivityType>();
        private PortalNotification _notification { get; set; }
        [Inject]
        private ILogger<ActivityLog> _logger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _profileId = _underwriters.First().Id;

            try
            {
                var underwriters = await _client.GetFromJsonAsync<IEnumerable<UnderwriterResponse>>("/api/admin/underwriting/underwriters");
                _underwriters.AddRange(underwriters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching underwriters");
            }
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if(firstRender)
            {
                _start = await _timezone.GetLocalDateTime(_start);
                _end = await _timezone.GetLocalDateTime(_end);
                await Update();
            }
        }

        private async Task Update()
        {
            try
            {
                var start = _start.ToQueryString();
                var end = _end.ToQueryString();
                var activities = await _client.GetFromJsonAsync<IEnumerable<ActivityResponse>>($"/api/admin/activity/list?profileId={_profileId}&start={start}&end={end}");
                _activities.ReplaceRange(activities.OrderByDescending(x => x.Date));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity list");
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task CreateActivityAsync()
        {
            var date = await _timezone.GetLocalFullDate();
            _newActivity = new ActivityResponse
            {
                Date = date.Date
            };
        }

        private void OnEditItem(GridCommandEventArgs args)
        {
            _updateActivity = args.Item as ActivityResponse;
        }

        private async Task OnSaveActivity()
        {
            if (_newActivity is null)
                return;

            using var response = await _client.PostAsJsonAsync("/api/admin/activity/create", _newActivity);

            _newActivity = null;

            if (response.IsSuccessStatusCode)
                _notification.ShowSuccess("New Activity Created");
            else
                _notification.ShowWarning("An error occurred while trying to create the activity");
            await Update();
        }

        private async Task OnUpdateActivity()
        {
            if (_updateActivity is null)
                return;

            using var response = await _client.PostAsJsonAsync("/api/admin/activity/update", _updateActivity);

            _updateActivity = null;

            if (response.IsSuccessStatusCode)
                _notification.ShowSuccess("Activity successfully updated");
            else
                _notification.ShowWarning("An error occurred while trying to update the activity");
            await Update();
        }

        private async Task OnDeleteItem(GridCommandEventArgs args)
        {
            var activity = args.Item as ActivityResponse;
            using var response = await _client.DeleteAsync($"/api/admin/activity/delete/{activity.Id}");

            switch(response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.NotFound:
                    _notification.ShowSuccess("Activity successfully deleted.");
                    break;
                case HttpStatusCode.Unauthorized:
                    _notification.ShowError("You may only delete your own activities.");
                    break;
                default:
                    _notification.ShowError("An error occurred while attempting to delete the activity");
                    break;
            }

            await Update();
        }
    }
}
