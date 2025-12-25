using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ConsoleAppHost;

// Minimal Generic Host example for a console app
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Bind configuration section to ApplicationSettings and register the service
        services.Configure<ApplicationSettings>(context.Configuration.GetSection(ApplicationSettings.SECTION_NAME));
        services.AddHostedService<ConsoleService>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
