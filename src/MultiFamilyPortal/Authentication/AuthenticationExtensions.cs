using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.SaaS.Authentication.Google;
using MultiFamilyPortal.SaaS.Authentication.MicrosoftAccount;

namespace MultiFamilyPortal.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new AuthenticationOptions();
            configuration.GetSection("Authentication").Bind(options);

            services.AddTransient<IClaimsTransformation, PortalClaimsTransformation>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PortalPolicy.AdminPortalViewer, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter, PortalRoles.Mentor, PortalRoles.BlogAuthor));
                options.AddPolicy(PortalPolicy.Blogger, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.BlogAuthor));
                options.AddPolicy(PortalPolicy.InvestorRelations, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter));
                options.AddPolicy(PortalPolicy.Underwriter, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter));
                options.AddPolicy(PortalPolicy.UnderwritingViewer, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter, PortalRoles.Mentor, PortalRoles.BlogAuthor));
                options.AddPolicy(PortalPolicy.InvestorPortalViewer, builder =>
                    builder.RequireRole(PortalRoles.Investor, PortalRoles.Sponsor));
            });

            services.AddIdentity<SiteUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
                .AddEntityFrameworkStores<MFPContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }
                    return Task.FromResult(0);
                };
            });

            services.AddAuthentication()
                .AddGoogle()
                .AddMicrosoftAccount();

            return services;
        }
    }
}
