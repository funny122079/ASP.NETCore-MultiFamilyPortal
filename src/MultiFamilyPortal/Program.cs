using MultiFamilyPortal.Extensions;

await WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .ConfigureApplication()
    .StartAndRunAsync();
