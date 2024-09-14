using System.Diagnostics;
using IMS.Core.Model;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IMS.Infrastructure.ScheduledJobs
{
    public class AuthServerPollingService : BackgroundService
    {
        private readonly ILogger<AuthServerPollingService> _logger;
        private readonly AuthServerContext _authServerContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

        public AuthServerPollingService(
            ILogger<AuthServerPollingService> logger,
            AuthServerContext context,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _authServerContext = context;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running periodic task at: {time}", DateTimeOffset.Now);

                // Call your service method here
                await DoWork(stoppingToken);

                // Wait for the interval or until the task is cancelled
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            // Implement your logic here
            _logger.LogInformation("Executing work at: {time}", DateTimeOffset.Now);

            List<AuthUserDTO>? authUserDTOs = await _authServerContext.PollUserData();
            if (authUserDTOs == null || !authUserDTOs.Any())
                return;

            using (var scope = _serviceProvider.CreateScope()) // this will use `IServiceScopeFactory` internally
            {
                var _dbContext = scope.ServiceProvider.GetService<DataBaseContext>();

                foreach (var authUser in authUserDTOs)
                {
                    User dbUser = await _dbContext
                        .users.AsNoTracking()
                        .FirstOrDefaultAsync(dbUser => dbUser.Email == authUser.email);

                    // If user is not found in the database, create a new user
                    if (dbUser == null)
                    {
                        User tempUserDTO = new User()
                        {
                            Email = authUser.email,
                            FirstName = authUser.firstName,
                            LastName = authUser.lastName,
                            ContactNumber = authUser.contactNumber,
                            Role = "Student",
                            IsActive = true,
                        };
                        _dbContext.users.Add(tempUserDTO);
                    }
                    // If user is found in the database and user data have changed, update the user
                    else if (
                        (dbUser.FirstName != authUser.firstName)
                        || (dbUser.LastName != authUser.lastName)
                        || (dbUser.ContactNumber != authUser.contactNumber)
                    )
                    {
                        dbUser.FirstName = authUser.firstName;
                        dbUser.LastName = authUser.lastName;
                        dbUser.ContactNumber = authUser.contactNumber;
                        _dbContext.users.Update(dbUser);
                    }
                }

                List<User> dbUserEntries = await _dbContext.users.ToListAsync();
                // If user is not found in the auth server, set the user to inactive
                foreach (var dbUser in dbUserEntries)
                {
                    if (!authUserDTOs.Any(authUser => authUser.email == dbUser.Email))
                    {
                        dbUser.IsActive = false;
                        _dbContext.users.Update(dbUser);
                    }
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
