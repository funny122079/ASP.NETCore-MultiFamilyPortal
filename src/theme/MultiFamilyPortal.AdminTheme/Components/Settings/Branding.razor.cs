using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Themes.Internals;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.DataSource;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class Branding
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<Branding> Logger { get; set; }

        [Inject]
        private IThemeFactory ThemeFactory { get; set; }

        private Logo _selected;
        private TelerikGrid<Logo> grid;
        private readonly List<string> _allowedFileTypes = new() { ".png", ".svg", ".jpeg",".jpg" };
        public string LogoUrl(string name) => ToAbsoluteUrl($"branding/{name}");
        private bool _showWindow = false;
        private readonly ObservableRangeCollection<Logo> _logos = new();
        private readonly IEnumerable<Logo> _globalLogos = new[]
        {
            new Logo
            {
                Type = "Global",
                DisplayName = "Browser Icon",
                Href = "/apple-touch-icon.png?",
                Name = "favicon",
                Size = "1024 x 1024"
            },
            new Logo
            {
                Type = "Global",
                DisplayName = "Default Logo",
                Href = "/theme/branding/logo?",
                Name = "logo",
                Size = "1024 x 1024"
            },
            new Logo
            {
                Type = "Global",
                DisplayName = "Dark Theme Logo",
                Href = "/theme/branding/logo-dark?",
                Name = "logo-dark",
                Size = "1024 x 1024"
            },
            new Logo
            {
                Type = "Global",
                DisplayName = "Default Logo - Horizontal",
                Href = "/theme/branding/logo-side?",
                Name = "logo-side",
                Size = "512 x 1024"
            },
            new Logo
            {
                Type = "Global",
                DisplayName = "Dark Theme Logo - Horizontal",
                Href = "/theme/branding/logo-dark-side?",
                Name = "logo-dark-side",
                Size = "512 x 1024"
            }
        };

        protected override void OnInitialized()
        {
            _logos.Clear();
            _logos.AddRange(_globalLogos);
            var theme = ThemeFactory.GetFrontendTheme();
            if(theme.Resources is not null && theme.Resources.Any())
            {
                _logos.AddRange(theme.Resources.Select(x => new Logo
                {
                    Type = "Theme",
                    DisplayName = x.Name,
                    Href = $"/theme/branding/resource?file={x.Name}&",
                    Name = x.Name,
                    Size = $"{x.Height} x {x.Width}"
                }));
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var desiredState = new GridState<Logo>
                {
                    GroupDescriptors = new[] {
                        new GroupDescriptor {
                            Member = nameof(Logo.Type),
                            MemberType = typeof(string)
                        }
                    }
                };
                await grid.SetState(desiredState);
            }
        }

        private string ToAbsoluteUrl(string url) => $"{_client.BaseAddress}api/admin/settings/{url}";

        private void UpdateLogo(GridCommandEventArgs args)
        {
            _selected = args.Item as Logo;
            _showWindow = true;
        }

        private void OnSuccessHandler(UploadSuccessEventArgs e)
        {
            if (e.Operation == UploadOperationType.Upload)
                _showWindow = false;
            else 
                Logger.LogWarning("Upload failure");
        }

        private record Logo
        {
            public string Type { get; init; }

            public string Href { get; init; }

            public string DisplayName { get; init; }

            public string Name { get; init; }

            public string Size { get; init; }
        }
    }
}
