using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.AdminTheme.Layouts
{
    public partial class NavMenu
    {
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal User { get; set; }

        [CascadingParameter]
        private IPortalTheme Theme { get; set; }
        private IMenuProvider MenuProvider => Theme as IMenuProvider;
        private RootMenuOption _selected;

        private bool collapseNavMenu = true;
        private bool collapseSubMenu = true;
        private bool optionSet;
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override void OnInitialized()
        {
            _navigationManager.LocationChanged += OnLocationChanged;
            var location = new Uri(_navigationManager.Uri);
            UpdateSelectedOption(location.AbsolutePath);
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            var location = new Uri(e.Location);
            _selected = null;
            if(MenuProvider is not null)
            {
                UpdateSelectedOption(location.AbsolutePath);
            }

            optionSet = false;
        }

        private void UpdateSelectedOption(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return;

            var selected = MenuProvider.Menu.FirstOrDefault(x => x.Link == uri || (x.Children is not null && x.Children.Any(c => uri.StartsWith(c.Link))));

            if (selected != null)
                SetOption(selected);
        }

        private void SetOption(RootMenuOption option)
        {
            if(!(option.Children is null || !option.Children.Any()))
            {
                if (option == _selected)
                    collapseSubMenu = !collapseSubMenu;
                else
                    collapseSubMenu = false;
            }

            _selected = option;
            optionSet = true;
        }

        private void ToggleNavMenu()
        {
            if(optionSet)
            {
                optionSet = false;
                return;
            }
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
