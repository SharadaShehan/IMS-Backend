using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team4.LTS.Core.Model;
using team4.LTS.Core.Model.DTO;
using Team4.LTS.Infra.Data;

namespace team4.LTS.Infra.Services
{
	public class UserService: IHostedService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly TimeSpan _interval = TimeSpan.FromMinutes(10); // Run every 10 minutes
		private Timer _timer;

		public UserService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(DoWork, null, TimeSpan.Zero, _interval);
			return Task.CompletedTask;
		}

		private void DoWork(object state)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

				// Simulated incoming data
				var users = new List<UserLogin>
			{
				new UserLogin { Email = "SmithJA@uoe.us", Password = "2345#abc", FirstName = "John", LastName = "Smith", ContactNumber = "+44234567890" },
                // ... add the rest of the users
            };

				foreach (var userLogin in users)
				{
					var existingUser = dbContext.users.SingleOrDefault(u => u.Email == userLogin.Email);

					if (existingUser == null)
					{
						// Add new user
						var newUser = new User
						{
							FirstName = userLogin.FirstName,
							LastName = userLogin.LastName,
							Email = userLogin.Email,
							IsActive = true, // Set this based on your logic
											 // Set other properties as needed
						};
						dbContext.users.Add(newUser);
					}
					else
					{
						// Update existing user
						existingUser.FirstName = userLogin.FirstName;
						existingUser.LastName = userLogin.LastName;
						existingUser.IsActive = true; // Set this based on your logic
													  // Update other properties as needed
					}
				}

				dbContext.SaveChanges();
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}
	}
}
