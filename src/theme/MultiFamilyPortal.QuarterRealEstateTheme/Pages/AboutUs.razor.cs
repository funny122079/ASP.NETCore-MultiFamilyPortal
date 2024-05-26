using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages
{
    public partial class AboutUs
    {
        [Inject]
        private IMFPContext DbContext { get; set; } = default!;

        private IEnumerable<SiteUser> HighlightedUsers = Array.Empty<SiteUser>();

        protected override async Task OnInitializedAsync()
        {
            HighlightedUsers = await DbContext.HighlightedUsers
                .Include(x => x.User)
                .ThenInclude(x => x.SocialLinks)
                .ThenInclude(x => x.SocialProvider)
                .Select(x => x.User)
                .ToArrayAsync();
        }
    }
}
