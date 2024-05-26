using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class PortalDbSettings
    {
        [Inject]
        private HttpClient Client { get; set; }

        private DbSettings Model;
        private PortalNotification notification;

        private int step;
        private bool showPrevious => step > 0;
        private bool showNext => step < 2;

        protected override async Task OnInitializedAsync()
        {
            var settings = await Client.GetFromJsonAsync<IEnumerable<Setting>>("/api/admin/settings");
            Model = new DbSettings(settings);
        }

        private void Next()
        {
            step++;
        }

        private void Previous()
        {
            if(step > 0)
                step--;
        }

        private async Task UpdateSettings()
        {
            bool failed = false;
            foreach(var setting in Model.UpdatedSettings())
            {
                using var result = await Client.PostAsJsonAsync($"/api/admin/settings/save/{setting.Key}", setting);
                if(!result.IsSuccessStatusCode)
                    failed = true;
            }

            if(failed)
            {
                notification.ShowWarning("Unable to save settings");
            }
            else
            {
                notification.ShowSuccess("Settings successfully updated");
            }
        }
    }
}
