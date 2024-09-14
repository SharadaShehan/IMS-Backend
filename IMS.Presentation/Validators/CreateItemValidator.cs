using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class CreateItemValidator : AbstractValidator<CreateItemDTO>
    {
        private readonly string textPattern = @"^.{2,20}$";
        private readonly string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";

        public CreateItemValidator()
        {
            // Validate equipmentId
            RuleFor(x => x.equipmentId)
                .GreaterThan(0)
                .WithMessage("Invalid Equipment Id. Equipment Id must be a positive integer.");

            // Validate serialNumber
            RuleFor(x => x.serialNumber)
                .NotEmpty()
                .WithMessage("Serial Number is required.")
                .Matches(textPattern)
                .WithMessage("Invalid Serial Number. Must be between 5 and 30 characters.");
        }
    }
}
