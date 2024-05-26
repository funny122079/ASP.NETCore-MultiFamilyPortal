using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Mail;
using AvantiPoint.EmailService.Postmark;
using BlazorAnimation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiFamilyPortal.Configuration;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Http;
using MultiFamilyPortal.SaaS;
using MultiFamilyPortal.SaaS.Extensions;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Extensions
{
    public static class CoreServicesRegistrationExtensions
    {
        public static IServiceCollection AddCorePortalServices(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new SiteConfiguration();
            configuration.Bind(config);
            configuration.Get<SiteConfiguration>();
            services.AddHttpContextAccessor()
                .AddSingleton(config)
                .AddSaaSApplication(configuration)
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IBlogSubscriberService, BlogSubscriberService>()
                .AddScoped<IBlogAdminService, BlogAdminService>()
                .AddScoped<ITemplateProvider, DatabaseTemplateProvider>()
                .AddScoped<IIpLookupService, IpLookupService>()
                .AddEmailValidationService()
                .AddPostmarkEmailService(ConfigurePostmarkEmailService)
                .AddScoped<IFormService, FormService>()
                .AddScoped<ITimeZoneService, TimeZoneService>()
                .AddScoped<ISiteInfo, SiteInfo>()
                .AddScoped<IStartupTask, BrandStartupTask>()
                .AddScoped<IBrandService, BrandService>()
                .AddScoped<IUnderwritingService, UnderwritingService>()
                .AddScoped<IReportGenerator, ReportGenerator>()
                .AddScoped(sp =>
                {
                    var context = sp.GetRequiredService<ITenantSettingsContext>();
                    return new GoogleCaptchaOptions
                    {
                        SecretKey = context.GetSetting<string>(PortalSetting.CaptchaSiteSecret),
                        SiteKey = context.GetSetting<string>(PortalSetting.CaptchaSiteKey)
                    };
                });

            if (string.IsNullOrEmpty(config?.Storage?.ConnectionString))
            {
                services.AddLocalFileStorage((sp, o) =>
                {
                    var tenantProvider = sp.GetRequiredService<ITenantProvider>();
                    var tenant = tenantProvider.GetTenant();
                    o.GetOutputPath = path => Path.Combine(tenant.Host, path);
                });
            }
            else
            {
                services.AddAzureBlobStorage((sp, o) =>
                {
                    if (string.IsNullOrEmpty(o.Container))
                        o.Container = "tenants";

                    var tenantProvider = sp.GetRequiredService<ITenantProvider>();
                    var tenant = tenantProvider.GetTenant();
                    o.GetOutputPath = path => Path.Combine(tenant.Host, path);
                });
            }

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.Configure<AnimationOptions>("Page", options =>
            {
                options.Effect = Effect.FadeIn;
                options.Speed = Speed.Fast;
                options.Delay = TimeSpan.FromMilliseconds(100);
                options.IterationCount = 1;
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            // Setup HttpClient for server side in a client side compatible fashion
            services.AddScoped<BlazorAuthenticationHandler>();
            services.AddScoped<HttpClient>(s =>
            {
                // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                var uriHelper = s.GetRequiredService<NavigationManager>();
                var handler = s.GetRequiredService<BlazorAuthenticationHandler>();
                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(uriHelper.BaseUri)
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            });

            return services;
        }

        private static void ConfigurePostmarkEmailService(IServiceProvider sp, PostmarkEmailServiceOptions options)
        {
            var config = sp.GetRequiredService<SiteConfiguration>();
            options.ApiKey = config.PostmarkApiKey;
            var context = sp.GetRequiredService<IMFPContext>();
            options.GetDefaultFromEmail = async () =>
            {
                var fromEmail = await context.GetSettingAsync<string>(PortalSetting.NotificationEmail);
                var fromEmailName = await context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
                return new MailAddress(fromEmail, fromEmailName);
            };
            options.GetDefaultToEmail = async () =>
            {
                var toEmail = await context.GetSettingAsync<string>(PortalSetting.ContactEmail);
                var fromEmailName = await context.GetSettingAsync<string>(PortalSetting.NotificationEmailFrom);
                return new MailAddress(toEmail, fromEmailName);
            };
        }
    }
}
