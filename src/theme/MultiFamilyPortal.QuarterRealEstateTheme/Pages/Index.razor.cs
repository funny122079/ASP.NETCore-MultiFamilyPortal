using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data.Views;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages
{
    public partial class Index
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default!;

        [Inject]
        private IMFPContext DbContext { get; set; } = default!;

        [Inject]
        private IBlogContext BlogContext { get; set; } = default!;

        private IEnumerable<InvestorTestimonial> testimonials = Array.Empty<InvestorTestimonial>();
        private IEnumerable<PostSummaryView> recentPosts = Array.Empty<PostSummaryView>();

        protected override async Task OnInitializedAsync()
        {
            recentPosts = await BlogContext.PostSummaryViews
                .Where(x => x.IsPublished && x.Published < DateTimeOffset.Now)
                .OrderByDescending(x => x.Published)
                .Take(5)
                .ToArrayAsync();

            testimonials = await DbContext.InvestorTestimonials
                .Where(x => x.Active == true)
                .ToArrayAsync();
        }
    }
}
