using System.Diagnostics;
using IMS.Infrastructure.ScheduledJobs;
using IMS.Infrastructure.Services;
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
