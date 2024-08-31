using Microsoft.Extensions.Hosting;
using IMS.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IMS.Infrastructure.ScheduledJobs
{
	public class AuthServerPollingService: BackgroundService
    {
        private readonly ILogger<AuthServerPollingService> _logger;
        private readonly AuthServerContext _authServerContext;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public AuthServerPollingService(ILogger<AuthServerPollingService> logger, AuthServerContext context)
        {
            _logger = logger;
            _authServerContext = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running periodic task at: {time}", DateTimeOffset.Now);
                List<DBUserDTO>? dBUserDTOs = await _authServerContext.PollUserData();
                if (dBUserDTOs != null && dBUserDTOs.Any()) {
                    foreach (var user in dBUserDTOs)
                    {
                        Debug.WriteLine(user.email);
                    }
                }
                // Call your service method here
                await DoWork(stoppingToken);

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private Task DoWork(CancellationToken stoppingToken)
        {
            // Implement your logic here
            _logger.LogInformation("Executing work at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }
    }
}
