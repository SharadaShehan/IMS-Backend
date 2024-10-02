using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class PreSignedUrlGenValidator : AbstractValidator<PresignedUrlRequestDTO>
    {
        private readonly List<string> validExtensions = new List<string>
        {
            "png",
            "jpg",
            "jpeg",
            "webp",
        };

        public PreSignedUrlGenValidator()
        {
            RuleFor(x => x.extension)
                .NotEmpty()
                .WithMessage("Extension is required.")
                .Must(extension => validExtensions.Contains(extension))
                .WithMessage("Invalid Extension. Only png, jpg, jpeg, and webp are allowed.");
        }
    }
}
