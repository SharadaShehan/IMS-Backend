using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.ScheduledJobs;
using IMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IMS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database Server Service
        var connectionString = configuration.GetConnectionString("DBConnection");
        services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(connectionString));

        // Repositories for use in Infrastructure Layer
        services.AddScoped<UserRepository, UserRepository>();
        services.AddScoped<LabRepository, LabRepository>();
        services.AddScoped<EquipmentRepository, EquipmentRepository>();
        services.AddScoped<ItemRepository, ItemRepository>();
        services.AddScoped<MaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<ReservationRepository, ReservationRepository>();

        // Authentication Server Service
        AuthServerContextOptions authServerOptions = new AuthServerContextOptions()
        {
            Endpoint = configuration.GetSection("AuthenticationServer")["Endpoint"],
            ClientId = configuration.GetSection("AuthenticationServer")["ClientId"],
            ClientSecret = configuration.GetSection("AuthenticationServer")["ClientSecret"],
        };
        services.AddSingleton<AuthServerContext>(sp => new AuthServerContext(authServerOptions));

        // Authentication Server Polling Job
        //services.AddHostedService<AuthServerPollingService>();

        EmailServiceContextOptions serviceOptions = new EmailServiceContextOptions()
        {
            APIKey = configuration.GetSection("EmailClient")["APIKey"],
            SenderEmail = configuration.GetSection("EmailClient")["SenderEmail"]
        };
        services.AddSingleton<EmailService>(sp => new EmailService(serviceOptions));

        // Notifitication Service Periodic Job
        //services.AddHostedService<SendNotificationsService>();

        // Set the service provider for the service locator
        ServiceLocator.SetServiceProvider(services.BuildServiceProvider());

        // Blob Storage Service
        services.AddSingleton<IBlobStorageClient>(sp =>
        {
            var connectionString = configuration.GetSection("AzureBlobStorage")["ConnectionString"];
            var containerName = configuration.GetSection("AzureBlobStorage")["ContainerName"];
            return new BlobStorageClient(connectionString, containerName);
        });
    }
}
