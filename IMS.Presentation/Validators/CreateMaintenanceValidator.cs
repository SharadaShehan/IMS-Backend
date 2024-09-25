using System.Text.RegularExpressions;
using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class CreateMaintenanceValidator : AbstractValidator<CreateMaintenanceDTO>
    {
        private readonly string datePattern = @"^\d{4}-\d{2}-\d{2}$";
        private readonly string descriptionPattern = @"^.{1,100}$";

        public CreateMaintenanceValidator()
        {
            // Validate itemId
            RuleFor(x => x.itemId)
                .GreaterThan(0)
                .WithMessage("Invalid Item Id. Item Id must be a positive integer.");

            // Validate technicianId
            RuleFor(x => x.technicianId)
                .GreaterThan(0)
                .WithMessage("Invalid Technician Id. Technician Id must be a positive integer.");

            // Validate startDate
            RuleFor(x => x.startDate)
                .Must(x => Regex.IsMatch(x, datePattern))
                .WithMessage("Invalid Start Date. The date format must be YYYY-MM-DD.")
                .NotEmpty()
                .WithMessage("Start Date is required.");

            // Validate endDate
            RuleFor(x => x.endDate)
                .Must(x => Regex.IsMatch(x, datePattern))
                .WithMessage("Invalid End Date. The date format must be YYYY-MM-DD.")
                .NotEmpty()
                .WithMessage("End Date is required.");

            // Validate endDate is greater than startDate
            When(
                x =>
                    (
                        Regex.IsMatch(x.startDate, datePattern)
                        && Regex.IsMatch(x.endDate, datePattern)
                    ),
                () =>
                {
                    RuleFor(x => x)
                        .Must(x => DateTime.Parse(x.endDate) > DateTime.Parse(x.startDate))
                        .WithMessage("End Date must be greater than Start Date.");
                }
            );

            // Validate taskDescription
            RuleFor(x => x.taskDescription)
                .NotEmpty()
                .WithMessage("Task Description is required.")
                .Matches(descriptionPattern)
                .WithMessage("Invalid Task Description. Must be between 1 and 100 characters.");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default(DateTime);
        }
    }
}
