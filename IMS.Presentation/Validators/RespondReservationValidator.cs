using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class RespondReservationValidator : AbstractValidator<RespondReservationDTO>
    {
        public RespondReservationValidator()
        {
            RuleFor(x => x.accepted).NotNull().WithMessage("Accepted status is required.");

            When(
                x => !x.accepted,
                () =>
                {
                    RuleFor(x => x.rejectNote)
                        .NotEmpty()
                        .WithMessage("Reject Note is required when reservation is rejected.")
                        .Matches(@"^.{1,100}$")
                        .WithMessage("Invalid Reject Note. Must be between 1 and 100 characters.");
                }
            );

            When(
                x => x.accepted,
                () =>
                {
                    RuleFor(x => x.itemId)
                        .NotNull()
                        .WithMessage("Item Id is required when reservation is accepted.")
                        .GreaterThan(0)
                        .WithMessage("Invalid Item Id.");
                }
            );
        }
    }
}
