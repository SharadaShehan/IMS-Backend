using System.Text.RegularExpressions;
using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class UpdateEquipmentValidator : AbstractValidator<UpdateEquipmentDTO>
    {
        private readonly string textPattern = @"^.{2,20}$";
        private readonly string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";

        public UpdateEquipmentValidator()
        {
            // Validate name (optional)
            RuleFor(x => x.name)
                .Matches(textPattern)
                .WithMessage("Invalid Equipment Name. Must be between 2 and 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.name)); // Only validate if name is provided

            // Validate model (optional)
            RuleFor(x => x.model)
                .Matches(textPattern)
                .WithMessage("Invalid Equipment Model. Must be between 2 and 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.model)); // Only validate if model is provided

            // Validate imageURL (optional)
            RuleFor(x => x.imageURL)
                .Cascade(CascadeMode.Stop)
                .Must(x => string.IsNullOrEmpty(x) || Regex.IsMatch(x, imageUrlPattern))
                .WithMessage(
                    "Invalid Image URL. Must be a valid URL ending with png, jpg, jpeg, or webp."
                )
                .When(x => !string.IsNullOrEmpty(x.imageURL)); // Only validate if imageURL is provided

            // Validate specification (optional)
            RuleFor(x => x.specification)
                .Must(x => x == null || x.GetType() == typeof(string))
                .WithMessage("Invalid Specification. Must be a string.")
                .When(x => x.specification != null); // Only validate if specification is provided

            // Validate maintenanceIntervalDays (optional)
            RuleFor(x => x.maintenanceIntervalDays)
                .GreaterThan(0)
                .WithMessage("Maintenance Interval Days must be a positive integer.")
                .When(x => x.maintenanceIntervalDays.HasValue); // Only validate if maintenanceIntervalDays is provided
        }
    }
}
