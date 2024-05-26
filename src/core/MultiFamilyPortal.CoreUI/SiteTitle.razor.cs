using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class SiteTitle
    {
        [Parameter]
        public string Title { get; set; }

        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; } = default !;
        private string FormattedTitle()
        {
            var siteTitle = SiteInfo.Title;
            if (string.IsNullOrEmpty(siteTitle))
            {
                siteTitle = "MultiFamily Portal";
            }

            if (!string.IsNullOrEmpty(Title))
            {
                var sanitized = Title.Split('-')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .LastOrDefault();
                if(!string.IsNullOrEmpty(sanitized))
                    return $"{siteTitle} - {sanitized}";
            }

            return siteTitle;
        }
    }
}
