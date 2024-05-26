using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Pages.Content
{
    public partial class PageList
    {
        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<CustomContent> _content = new();

        protected override async Task OnInitializedAsync()
        {
            var content = await _client.GetFromJsonAsync<IEnumerable<CustomContent>>("/api/admin/content");
            _content.ReplaceRange(content);
        }
    }
}
