using System.Text.RegularExpressions;
using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class CreateEquipmentValidator : AbstractValidator<CreateEquipmentDTO>
    {
        private readonly string textPattern = @"^.{2,20}$";
        private readonly string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";

        public CreateEquipmentValidator()
        {
            // Validate name
            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("Equipment Name is required.")
                .Matches(textPattern)
                .WithMessage("Invalid Equipment Name. Must be between 2 and 20 characters.");

            // Validate model
            RuleFor(x => x.model)
                .NotEmpty()
                .WithMessage("Equipment Model is required.")
                .Matches(textPattern)
                .WithMessage("Invalid Equipment Model. Must be between 2 and 20 characters.");

            // Validate labId
            RuleFor(x => x.labId)
                .GreaterThan(0)
                .WithMessage("Invalid Lab Id. Lab Id must be a positive integer.");

            // Validate imageURL (optional)
            RuleFor(x => x.imageURL)
                .Cascade(CascadeMode.Stop)
                .Must(x => string.IsNullOrEmpty(x) || Regex.IsMatch(x, imageUrlPattern))
                .WithMessage(
                    "Invalid Image URL. Must be a valid URL ending with png, jpg, jpeg, or webp."
                )
                .When(x => !string.IsNullOrEmpty(x.imageURL));

            // Validate specification (optional)
            RuleFor(x => x.specification)
                .Must(x => x == null || x.GetType() == typeof(string))
                .WithMessage("Invalid Specification. Must be a string.")
                .When(x => x.specification != null);

            // Validate maintenanceIntervalDays (optional)
            RuleFor(x => x.maintenanceIntervalDays)
                .GreaterThan(0)
                .When(x => x.maintenanceIntervalDays.HasValue)
                .WithMessage("Maintenance Interval Days must be a positive integer.")
                .When(x => x.maintenanceIntervalDays.HasValue);
        }
    }
}
