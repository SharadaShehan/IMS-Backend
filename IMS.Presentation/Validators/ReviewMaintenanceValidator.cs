using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class ReviewMaintenanceValidator : AbstractValidator<ReviewMaintenanceDTO>
    {
        private readonly string notePattern = @"^.{1,100}$";

        public ReviewMaintenanceValidator()
        {
            // Validate reviewNote only when 'accepted' is false
            RuleFor(x => x.reviewNote)
                .Matches(notePattern)
                .WithMessage("Invalid Review Note. Must be between 1 and 100 characters.")
                .When(x => !x.accepted); // Only validate if accepted is false

            // Validate accepted
            RuleFor(x => x.accepted).NotNull().WithMessage("Accepted must be true or false.");
        }
    }
}
