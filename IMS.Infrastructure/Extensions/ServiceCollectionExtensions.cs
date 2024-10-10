using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IMS.Infrastructure.ScheduledJobs;

namespace IMS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(
        this IServiceCollection services
    )
    {
        // Database Server Service
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (connectionString == null)
        {
            throw new Exception("Database Connection String not found in environment variables.");
        }
        services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(connectionString));

        // Repositories for use in Infrastructure Layer
        services.AddScoped<UserRepository, UserRepository>();
        services.AddScoped<LabRepository, LabRepository>();
        services.AddScoped<EquipmentRepository, EquipmentRepository>();
        services.AddScoped<ItemRepository, ItemRepository>();
        services.AddScoped<MaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<ReservationRepository, ReservationRepository>();

        // Authentication Server Service
        var authServerEndpoint = Environment.GetEnvironmentVariable("AUTH_SERVER_ENDPOINT");
        var authServerClientId = Environment.GetEnvironmentVariable("AUTH_SERVER_CLIENT_ID");
        var authServerClientSecret = Environment.GetEnvironmentVariable("AUTH_SERVER_CLIENT_SECRET");
        if (authServerEndpoint == null || authServerClientId == null || authServerClientSecret == null)
        {
            throw new Exception("Authentication Server Endpoint, Client ID, or Client Secret not found in environment variables.");
        }
        AuthServerContextOptions authServerOptions = new AuthServerContextOptions()
        {
            Endpoint = authServerEndpoint,
            ClientId = authServerClientId,
            ClientSecret = authServerClientSecret
        };
        services.AddSingleton<AuthServerContext>(sp => new AuthServerContext(authServerOptions));

        // Authentication Server Polling Job
        //services.AddHostedService<AuthServerPollingService>();
        
        // Email Service
        var emailClientApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var emailClientSenderEmail = Environment.GetEnvironmentVariable("SENDGRID_SENDER_EMAIL");
        if (emailClientApiKey == null || emailClientSenderEmail == null)
        {
            throw new Exception("Email Client API Key or Sender Email not found in environment variables.");
        }
        EmailServiceContextOptions serviceOptions = new EmailServiceContextOptions()
        {
            APIKey = emailClientApiKey,
            SenderEmail = emailClientSenderEmail
        };
        services.AddSingleton<EmailService>(sp => new EmailService(serviceOptions));

        // Notifitication Service Periodic Job
        //services.AddHostedService<SendNotificationsService>();

        // Set the service provider for the service locator
        ServiceLocator.SetServiceProvider(services.BuildServiceProvider());

        // Blob Storage Service
        services.AddSingleton<IBlobStorageClient>(sp =>
        {
            var connectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING");
            var containerName = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONTAINER_NAME");
            if (connectionString == null || containerName == null)
            {
                throw new Exception("Blob Storage Connection String or Container Name not found in environment variables.");
            }
            return new BlobStorageClient(connectionString, containerName);
        });
    }
}
