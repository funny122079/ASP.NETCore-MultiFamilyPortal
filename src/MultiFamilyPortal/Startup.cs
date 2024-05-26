using Microsoft.AspNetCore.Components.Authorization;
using MultiFamilyPortal.Areas.Identity;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Extensions;
using MultiFamilyPortal.Infrastructure;
using MultiFamilyPortal.SaaS.Extensions;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddMFContext(configuration)
                .AddDatabaseDeveloperPageExceptionFilter()
                .AddAuthenticationProviders(configuration)
                .AddCorePortalServices(configuration)
                .AddScoped<IStartupTask, PortalStartup>()
                .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<SiteUser>>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTelerikBlazor();
            services.RegisterThemes(env);

            // These have to be manually registered to avoid being linked out
            services.RegisterTheme<QuarterRealEstateTheme.QuarterRealEstateTheme>()
                .RegisterTheme<AdminTheme.AdminTheme>()
                .RegisterTheme<PortalTheme.PortalTheme>()
                .RegisterTheme<InvestorPortal.InvestorTheme>()
                .RegisterTheme<SuspendedTenantTheme.SuspendedTenantTheme>();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.MaxAge = TimeSpan.FromDays(7);
            });
        }

        public static void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for
                // production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMissingTenant();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
