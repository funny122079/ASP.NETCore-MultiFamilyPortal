using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using Telerik.Blazor.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class SiteThemes
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<SiteTheme> _themes = new();

        private PortalNotification notification;

        protected override Task OnInitializedAsync() => Update();

        private async Task Update()
        {
            _themes.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<SiteTheme>>("/api/admin/settings/themes"));
        }

        private async Task SetDefault(GridCommandEventArgs args)
        {
            var theme = args.Item as SiteTheme;
            using var response = await _client.PostAsJsonAsync("/api/admin/settings/themes/default", theme);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess($"Site Theme successfully updated to {theme.Id}");
            else
                notification.ShowError("An error occurred while attempting to update the default theme");

            await Update();
        }

    }
}
