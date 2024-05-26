using MultiFamilyPortal.SaaSAdmin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.ConfigureApp(app);

app.Run();
