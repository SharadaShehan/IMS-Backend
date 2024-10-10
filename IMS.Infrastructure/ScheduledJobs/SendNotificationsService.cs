using IMS.Application.DTO;
using IMS.Infrastructure.Repositories;
using IMS.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IMS.Infrastructure.ScheduledJobs
{
    public class SendNotificationsService : BackgroundService
    {
        private readonly ILogger<SendNotificationsService> _logger;
        private readonly EmailService _emailService;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromDays(1);

        public SendNotificationsService(
            ILogger<SendNotificationsService> logger,
            IServiceProvider serviceProvider,
            EmailService emailService
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running periodic task at: {time}", DateTimeOffset.Now);

                // Call your service method here
                await RemindPendingMaintenances(stoppingToken);
                await RemindDueItemReservations(stoppingToken);

                // Wait for the interval or until the task is cancelled
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task RemindPendingMaintenances(CancellationToken stoppingToken)
        {
            // Implement your logic here
            _logger.LogInformation("Executing work at: {time}", DateTimeOffset.Now);

            string subject = "Pending Maintenance Reminder";
            string plainTextContent = "";
            string htmlContent = "<h2>Pending Maintenance Items</h2><ul>";

            using (var scope = _serviceProvider.CreateScope()) // this will use `IServiceScopeFactory` internally
            {
                MaintenanceRepository? _maintenanceRepository = scope.ServiceProvider.GetService<MaintenanceRepository>();
                UserRepository? _userRepository = scope.ServiceProvider.GetService<UserRepository>();
                if (_maintenanceRepository == null || _userRepository == null)
                {
                    _logger.LogError("Failed to inject maintenanceRepository/userRepository");
                    return;
                }

                List<PendingMaintenanceDTO> maintenances = _maintenanceRepository.GetAllPendingMaintenanceDTOs();
                if (maintenances == null || maintenances.Count == 0) return;

                foreach (var maintenance in maintenances)
                {
                    htmlContent += $@"
                        <li>
                            <h3>{maintenance?.itemName} ({maintenance?.itemModel})</h3>
                            <p>Laboratory : {maintenance?.labName}</p>
                            <p>Serial Number : {maintenance?.itemSerialNumber}</p>
                            <p>Last Maintenance : {maintenance?.lastMaintenanceStartDate.ToString("dd MMM yyyy")} - {maintenance?.lastMaintenanceEndDate.ToString("dd MMM yyyy")}</p>
                        </li>";
                }
                htmlContent += "</ul>";

                List<UserDTO> clerks = _userRepository.GetAllClerkDTOs();

                foreach (var clerk in clerks)
                {
                    var success = await _emailService.SendEmail(clerk.email, subject, plainTextContent, htmlContent);
                    if (success)
                    {
                        _logger.LogInformation($@"Pending maintenance list successfully sent to Office Clerk ({clerk.firstName} {clerk.lastName})");
                    }
                    else
                    {
                        _logger.LogError($@"Failed to send pending maintenance list to Office Clerk ({clerk.firstName} {clerk.lastName})");
                    }
                }
            }
        }

        private async Task RemindDueItemReservations(CancellationToken stoppingToken)
        {
            // Implement your logic here
            _logger.LogInformation("Executing work at: {time}", DateTimeOffset.Now);

            string subject = "Reminder to Return Borrowed Item";
            string plainTextContent = "";

            using (var scope = _serviceProvider.CreateScope()) // this will use `IServiceScopeFactory` internally
            {
                ReservationRepository? _reservationRepository = scope.ServiceProvider.GetService<ReservationRepository>();
                if (_reservationRepository == null)
                {
                    _logger.LogError("Failed to inject reservationRepository");
                    return;
                }

                List<DueItemReservationDTO> reservations = _reservationRepository.GetAllDueItemReservationDTOs();
                if (reservations == null || reservations.Count == 0) return;

                foreach (var reservation in reservations)
                {

                    string htmlContent = $@"
                    <h2>Item Reservation Reminder</h2>
                    <p>Item : {reservation?.itemName} ({reservation?.itemModel})</p>
                    <p>Serial Number : {reservation?.itemSerialNumber}</p>
                    <p>Borrowed On : {reservation?.startDate.ToString("dd MMM yyyy")}</p>
                    <p>Due Date : {reservation?.endDate.ToString("dd MMM yyyy")}</p>";

                    if (reservation == null) continue;

                    var success = await _emailService.SendEmail(reservation.reservedUserEmail, subject, plainTextContent, htmlContent);
                    if (success)
                    {
                        _logger.LogInformation($@"Item reservation reminder successfully sent to {reservation?.reservedUserName}");
                    }
                    else
                    {
                        _logger.LogError($@"Failed to send item reservation reminder to {reservation?.reservedUserName}");
                    }
                }
            }
        }

        private async Task RemindPickupPendingItemReservations(CancellationToken stoppingToken)
        {
            // Implement your logic here
            _logger.LogInformation("Executing work at: {time}", DateTimeOffset.Now);

            string subject = "Reminder to Pickup Reserved Item";
            string plainTextContent = "";

            using (var scope = _serviceProvider.CreateScope()) // this will use `IServiceScopeFactory` internally
            {
                ReservationRepository? _reservationRepository = scope.ServiceProvider.GetService<ReservationRepository>();
                if (_reservationRepository == null)
                {
                    _logger.LogError("Failed to inject reservationRepository");
                    return;
                }

                List<DueItemReservationDTO> reservations = _reservationRepository.GetAllPickupPendingReservationDTOs();
                if (reservations == null || reservations.Count == 0) return;

                foreach (var reservation in reservations)
                {
                    string htmlContent = $@"
                    <h2>Item Reservation Pickup Reminder</h2>
                    <p>Item : {reservation?.itemName} ({reservation?.itemModel})</p>
                    <p>Serial Number : {reservation?.itemSerialNumber}</p>
                    <p>Reserved On : {reservation?.createdAt.ToString("dd MMM yyyy")}</p>
                    <p>Pickup Date : {reservation?.startDate.ToString("dd MMM yyyy")}</p>";

                    if (reservation == null) continue;

                    var success = await _emailService.SendEmail(reservation.reservedUserEmail, subject, plainTextContent, htmlContent);
                    if (success)
                    {
                        _logger.LogInformation($@"Item reservation pickup reminder successfully sent to {reservation?.reservedUserName}");
                    }
                    else
                    {
                        _logger.LogError($@"Failed to send item reservation pickup reminder to {reservation?.reservedUserName}");
                    }
                }
            }
        }

    }
}
