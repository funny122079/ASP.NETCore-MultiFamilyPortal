using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.Extensions
{
    public static class ThemeRegistrationExtensions
    {
        public static IServiceCollection RegisterThemes(this IServiceCollection services, IWebHostEnvironment env)
        {
            LoadModules(env);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.ExportedTypes.Any(t => typeof(IPortalTheme).IsAssignableFrom(t)))
                .SelectMany(x => x.ExportedTypes)
                .Where(x => !x.IsAbstract && typeof(IPortalTheme).IsAssignableFrom(x));

            foreach(var themeType in types)
            {
                services.RegisterTheme(themeType);
            }
            return services;
        }

        private static void LoadModules(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "App_Data", "modules");
            if(!Directory.Exists(path))
                return;

            var dlls = Directory.GetFiles(path, "*.dll");
            try
            {
                foreach(var dllPath in dlls)
                {
                    var bytes = File.ReadAllBytes(dllPath);
                    AppDomain.CurrentDomain.Load(bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
