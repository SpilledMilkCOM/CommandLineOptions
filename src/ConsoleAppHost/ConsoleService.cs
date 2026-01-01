using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleAppHost
{
    public sealed class ConsoleService : BackgroundService
    {
        private readonly ILogger<ConsoleService> _logger;
        private readonly IOptions<ApplicationSettings> _options;

        public ConsoleService(ILogger<ConsoleService> logger, IOptions<ApplicationSettings> options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ConsoleService starting with Message='{Message}', Verbose={Verbose}, LoopCount={LoopCount}", 
                _options.Value.Message, _options.Value.Verbose, _options.Value.LoopCount);

            int count = 0;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("{Time}: {Message}", DateTimeOffset.Now, _options.Value.Message);
                
                count++;

                if (_options.Value.LoopCount > 0 && count >= _options.Value.LoopCount)
                {
                    _logger.LogInformation("Reached loop count of {LoopCount}, stopping", _options.Value.LoopCount);
                    break;
                }

                await Task.Delay(_options.Value.Verbose ? 1000 : 5000, stoppingToken);
            }

            _logger.LogInformation("ConsoleService stopping");
        }
    }
}
