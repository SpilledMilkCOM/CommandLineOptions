using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleAppHost
{
    public sealed class ConsoleService : BackgroundService
    {
        private readonly ILogger<ConsoleService> _logger;
        private readonly ApplicationSettings _settings;
        private readonly IHostApplicationLifetime _lifetime;

        public ConsoleService(ILogger<ConsoleService> logger
                            , IOptions<ApplicationSettings> options
                            , IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _settings = options.Value;
            _lifetime = lifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ConsoleService starting with Message='{Message}', Verbose={Verbose}, LoopCount={LoopCount}", 
                _settings.Message, _settings.Verbose, _settings.LoopCount);

            int count = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("{Time}: {Message}", DateTimeOffset.Now, _settings.Message);
                
                count++;

                if (_settings.LoopCount > 0 && count >= _settings.LoopCount)
                {
                    _logger.LogInformation("Reached loop count of {LoopCount}, stopping", _settings.LoopCount);
                    _lifetime.StopApplication();
                    
                    break;
                }

                await Task.Delay(_settings.Verbose ? 1000 : 5000, stoppingToken);
            }

            _logger.LogInformation("ConsoleService stopping");
        }
    }
}
