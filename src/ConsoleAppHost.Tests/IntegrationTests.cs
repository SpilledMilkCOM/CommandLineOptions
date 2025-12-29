using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleAppHost.Tests.Utilities;

namespace ConsoleAppHost.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public async Task WorkerLogsConfiguredMessage()
        {
            var logs = new ConcurrentQueue<string>();

            var provider = new InMemoryLoggerProvider(logs);

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new KeyValuePair<string, string?>[]
                    {
                        new KeyValuePair<string, string?>($"{ApplicationSettings.SECTION_NAME}:Message", "Test message"),
                        new KeyValuePair<string, string?>($"{ApplicationSettings.SECTION_NAME}:Verbose", "true"),
                        new KeyValuePair<string, string?>("Logging:LogLevel:Default", "Information")
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ApplicationSettings>(context.Configuration.GetSection(ApplicationSettings.SECTION_NAME));
                    services.AddHostedService<ConsoleService>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(provider);
                })
                .Build();

            await host.StartAsync();

            // Wait long enough for the ConsoleService to write at least one message
            await Task.Delay(1500);

            await host.StopAsync();

            Assert.IsTrue(logs.Any(s => s.Contains("Test message")));
        }
    }
}
