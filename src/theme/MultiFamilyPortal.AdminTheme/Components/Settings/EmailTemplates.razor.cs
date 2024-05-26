using System.Collections;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class EmailTemplates
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private ObservableRangeCollection<EmailTemplate> _emailTemplates { get; set; } = new();

        private EmailTemplate _selected;
        private PortalNotification notification;
        private IEnumerable<TemplateVariableDefinition> _definitions;

        protected override async Task OnInitializedAsync()
        {
            await UpdateAsync();
        }

        private void OnEditItem(GridCommandEventArgs args)
        {
            _definitions = Array.Empty<TemplateVariableDefinition>();
            _selected = args.Item as EmailTemplate;

            _selected.Html = Regex.Replace(_selected.Html, Regex.Escape("{{SiteUrl}}"), _navigationManager.BaseUri);

            var type = Type.GetType(_selected.Model);
            if (type is null)
                return;

            _definitions = type.GetRuntimeProperties()
                .Select(x => new TemplateVariableDefinition
                {
                    Name = x.Name,
                    PartialTemplate = x.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)
                });
        }

        private async Task OnUpdateTemplate()
        {
            if (_selected is null)
                return;

            _selected.Html = Regex.Replace(_selected.Html, Regex.Escape(_navigationManager.BaseUri), "{{SiteUrl}}");
            using var response = await _client.PostAsJsonAsync("/api/admin/settings/email-templates/update", _selected);

            if (response.IsSuccessStatusCode)
            {
                notification.ShowSuccess("Email template was successfully updated");
                _selected = null;
                await UpdateAsync();
            }
            else
            {
                notification.ShowError("Unable to update the email template");
            }
        }

        private async Task UpdateAsync()
        {
            _emailTemplates.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<EmailTemplate>>("/api/admin/settings/email-templates"));
        }

        private record TemplateVariableDefinition
        {
            public string Name { get; init; }
            public bool PartialTemplate { get; init; }
        }
    }
}
