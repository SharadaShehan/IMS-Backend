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
        private readonly string imageNamePattern = @"^.{5,20}$";

        public PreSignedUrlGenValidator()
        {
            RuleFor(x => x.imageName)
                .NotEmpty()
                .WithMessage("Image Name is required.")
                .Matches(imageNamePattern)
                .WithMessage("Invalid Image Name. Must be between 5 and 20 characters.");

            RuleFor(x => x.extension)
                .NotEmpty()
                .WithMessage("Extension is required.")
                .Must(extension => validExtensions.Contains(extension))
                .WithMessage("Invalid Extension. Only png, jpg, jpeg, and webp are allowed.");
        }
    }
}
