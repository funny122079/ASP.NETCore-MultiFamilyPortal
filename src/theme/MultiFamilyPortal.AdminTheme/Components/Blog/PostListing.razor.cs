using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Views;

namespace MultiFamilyPortal.AdminTheme.Components.Blog
{
    public partial class PostListing
    {
        [Parameter]
        public PostListView Post { get; set; }

        public string Link { get; set; }

        private string GetStatus(PostListView post)
        {
            if (post is null)
                return null;
            if (post.IsPublished == false)
                return "Draft";
            else if (post.Published > DateTimeOffset.Now)
                return "Scheduled";
            return "Published";
        }

        protected override void OnInitialized()
        {
            Link = $"/post/{Post?.Id}";
        }
    }
}
