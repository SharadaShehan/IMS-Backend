using System.Text.RegularExpressions;
using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class UpdateLabValidator : AbstractValidator<UpdateLabDTO>
    {
        private readonly string textPattern = @"^.{2,20}$";
        private readonly string imageUrlPattern = @"^https?:\/\/.*\.(?:png|jpg|jpeg|webp)$";

        public UpdateLabValidator()
        {
            // Validate labName
            RuleFor(x => x.labName)
                .Must(x => (x == null) || Regex.IsMatch(x, textPattern))
                .WithMessage("Invalid Lab Name. Must be between 2 and 20 characters.")
                .When(x => x != null);

            // Validate labCode
            RuleFor(x => x.labCode)
                .Must(x => (x == null) || Regex.IsMatch(x, textPattern))
                .WithMessage("Invalid Lab Code. Must be between 2 and 20 characters.")
                .When(x => x != null);

            // Validate imageURL (optional)
            RuleFor(x => x.imageURL)
                .Must(x => (x == null) || Regex.IsMatch(x, imageUrlPattern))
                .WithMessage(
                    "Invalid Image URL. Must be a valid URL ending with png, jpg, jpeg, or webp."
                )
                .When(x => x != null);
        }
    }
}
