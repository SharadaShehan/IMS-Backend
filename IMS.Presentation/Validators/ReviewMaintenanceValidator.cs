using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class ReviewMaintenanceValidator : AbstractValidator<ReviewMaintenanceDTO>
    {
        private readonly string notePattern = @"^.{1,100}$";

        public ReviewMaintenanceValidator()
        {
            When(
                x => !(x.accepted),
                () =>
                {
                    // Validate reviewNote only when 'accepted' is false
                    RuleFor(x => x.reviewNote)
                        .NotEmpty()
                        .WithMessage("Review Note is required when maintenance is rejected.")
                        .Matches(notePattern)
                        .WithMessage("Invalid Review Note. Must be between 1 and 100 characters.");
                }
            );

            // Validate accepted
            RuleFor(x => x.accepted).NotNull().WithMessage("Accepted must be true or false.");
        }
    }
}
