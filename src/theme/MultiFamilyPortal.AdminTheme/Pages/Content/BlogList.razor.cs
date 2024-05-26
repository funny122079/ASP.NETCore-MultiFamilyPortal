using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Views;

namespace MultiFamilyPortal.AdminTheme.Pages.Content
{
    public partial class BlogList
    {
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<PostListView> _posts = new();

        protected override async Task OnInitializedAsync()
        {
            _posts.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<PostListView>>("/api/admin/blog/list"));
        }

        private void CreatePost()
        {
            _navigationManager.NavigateTo("/admin/create-post");
        }
    }
}
