#if DEBUG
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.SaaS;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using MultiFamilyPortal.SaaS.TenantProviders;

namespace MultiFamilyPortal.Infrastructure
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<MFPContext>
    {
        private const string DesignTimeConnectionString = "Server=(localdb)\\mssqllocaldb;Database={0};Trusted_Connection=True;MultipleActiveResultSets=true";
        public MFPContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(ConfigurationStream())
                .Build();
            var tenant = new Tenant
            {
                Created = DateTimeOffset.Now,
                DatabaseName = "MultiFamilyPortal",
                Host = "localhost:7009"
            };

            services
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(configuration.Get<DatabaseSettings>())
                .AddSingleton<ITenantProvider>(new StartupTenantProvider(tenant))
                .AddDbContext<MFPContext>(options =>
                    options.UseSqlServer(string.Format(DesignTimeConnectionString, tenant.DatabaseName)))
                .AddIdentity<SiteUser, IdentityRole>(options =>
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

            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<MFPContext>();
        }

        private static Stream ConfigurationStream()
        {
            var settings = new
            {
                ConnectionStrings = new Dictionary<string, string>
                {
                    { "DefaultConnection", DesignTimeConnectionString },
                    { "TenantConnection", string.Format(DesignTimeConnectionString, "TenantAdmin") }
                }
            };

            var data = JsonSerializer.SerializeToUtf8Bytes(settings);
            return new MemoryStream(data);
        }
    }
}
#endif
