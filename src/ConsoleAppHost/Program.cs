using CommandLineOptions;
using ConsoleAppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Minimal Generic Host example for a console app
var builder = Host.CreateDefaultBuilder(args);

// Parse command-line arguments
var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger<OptionsParser>();
var parser = new OptionsParser(logger);
var cmdLineSettings = parser.Parse<ApplicationSettings>(args);

var host = builder
    .ConfigureServices((context, services) =>
    {
        // Bind configuration section to ApplicationSettings and register the service
        services.Configure<ApplicationSettings>(settings =>
        {
            // Load from configuration first
            context.Configuration.GetSection(ApplicationSettings.SECTION_NAME).Bind(settings);

            // Override with command-line arguments
            settings.ApplyCommandLineOverrides(cmdLineSettings);
        });

        services.AddHostedService<ConsoleService>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
