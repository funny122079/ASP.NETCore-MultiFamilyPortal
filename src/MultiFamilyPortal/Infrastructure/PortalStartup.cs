using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data.Services;
using MultiFamilyPortal.FirstRun;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.Infrastructure
{
    public class PortalStartup : IStartupTask
    {
        private IEnumerable<IPortalFrontendTheme> _themes { get; }
        private IStartupContextHelper _contextHelper { get; }
        private IServiceProvider _serviceProvider { get; }

        public PortalStartup(IEnumerable<IPortalFrontendTheme> themes,
            IStartupContextHelper contextHelper)
        {
            _contextHelper = contextHelper;
            _themes = themes;
        }

        public async Task StartAsync()
        {
            await SeedThemes();
            await SeedRoles();

            await _contextHelper.RunUserManagerAction(async (userManager, tenant, services) =>
            {
                if (!await userManager.Users.AnyAsync())
                {
                    var configurationValidator = services.GetRequiredService<ISiteConfigurationValidator>();
                    configurationValidator.SetFirstRunTheme(new FirstRunTheme());
                }
            });
        }

        private async Task SeedThemes()
        {
            var defaultThemeName = nameof(PortalTheme.PortalTheme);
            await _contextHelper.RunDatabaseAction(async db =>
            {
                foreach (var theme in _themes)
                {
                    if (!await db.SiteThemes.AnyAsync(x => x.Id == theme.Name))
                    {
                        var siteTheme = new SiteTheme
                        {
                            Id = theme.Name,
                            IsDefault = theme.GetType().Name.Contains(defaultThemeName)
                        };
                        db.SiteThemes.Add(siteTheme);
                        await db.SaveChangesAsync();
                    }
                }

                if (!await db.SiteThemes.AnyAsync(x => x.IsDefault == true))
                {
                    var theme = await db.SiteThemes
                        .FirstOrDefaultAsync(x => x.Id == defaultThemeName);
                    theme.IsDefault = true;
                    db.SiteThemes.Update(theme);
                    await db.SaveChangesAsync();
                }
            });
        }

        private async Task SeedRoles()
        {
            var type = typeof(PortalRoles);
            var allRoles = type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => x.Name);

            await _contextHelper.RunDatabaseAction(async db =>
            {
                foreach (var roleName in allRoles)
                {
                    if (!await db.Roles.AnyAsync(x => x.Name == roleName))
                    {
                        await db.Roles.AddAsync(new IdentityRole(roleName)
                        {
                            NormalizedName = roleName.ToUpper()
                        });
                        await db.SaveChangesAsync();
                    }
                }
            });
        }
    }
}
