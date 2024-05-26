using Microsoft.AspNetCore.Components.Routing;
using MultiFamilyPortal.QuarterRealEstateTheme.Layouts;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.QuarterRealEstateTheme
{
    public class QuarterRealEstateTheme : IPortalFrontendTheme, IMenuProvider
    {
        private const string baseContentPath = "_content/MultiFamilyPortal.QuarterRealEstateTheme";
        public Type Layout { get; } = typeof(MainLayout);
        public string Name { get; } = "Quarter Real Estate";
        public Type _404 { get; } = typeof(Components._404);
        public string[] RequiredStyles { get; } = new[] { $"{baseContentPath}/css/style.css" };
        public ThemeResource[] Resources { get; } = new[]
        {
            new ThemeResource
            {
                Height = 773,
                Width = 870,
                Name = "default-home",
                Path = $"/{baseContentPath}/img/bg/default-home.png"
            }
        };

        public IEnumerable<RootMenuOption> Menu => new[]
        {
            new RootMenuOption
            {
                Title = "Home",
                IconClass = "fas fa-home",
                Link = "/",
                Match = NavLinkMatch.All
            },
            new RootMenuOption
            {
                Title = "About",
                IconClass = "fas fa-users",
                Link = "/about",
                Match = NavLinkMatch.All
            },
            new RootMenuOption
            {
                Title = "Strategy",
                IconClass = "fas fa-chess-queen-alt",
                Link = "/strategy",
                Match = NavLinkMatch.All
            },
            new RootMenuOption
            {
                Title = "Contact",
                IconClass = "fal fa-at",
                Link = "/contact",
                Match = NavLinkMatch.All
            }
        };
    }
}
