using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Models;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class Reminders
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<Reminders> _logger { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private IEnumerable<ContactReminder> _reminders = Array.Empty<ContactReminder>();
        private ContactReminder _selectedReminder;
   
        protected override async Task OnInitializedAsync()
        {
            await GetRemindersAsync();
        }

        private async Task GetRemindersAsync()
        {
            try
            {
                _reminders = await _client.GetFromJsonAsync<IEnumerable<ContactReminder>>("/api/admin/dashboard/reminders");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching reminders : " + DateTimeOffset.UtcNow);
            }
        }

        private void RedictToContact(Guid id)
        {
            _navigationManager.NavigateTo($"/admin/contacts/detail/{id}");
        }

        private async Task DismissReminder(ContactReminder reminder, bool state)
        {
            _selectedReminder = reminder;
            _selectedReminder.Dismissed = state;
            try
            {
               await _client.PutAsJsonAsync($"/api/admin/dashboard/reminders/{_selectedReminder.Id}", _selectedReminder);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error dismissing reminder : " + DateTimeOffset.UtcNow);
            }
            _selectedReminder = null;
        }
         
        private string GetFullName(ContactReminder reminder)
        {
            return reminder.Contact?.FirstName + " " + reminder.Contact?.LastName;
        }
    }
}