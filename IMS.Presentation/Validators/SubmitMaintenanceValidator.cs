using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class SubmitMaintenanceValidator : AbstractValidator<SubmitMaintenanceDTO>
    {
        private readonly string notePattern = @"^.{1,100}$";

        public SubmitMaintenanceValidator()
        {
            // Validate submitNote (optional)
            RuleFor(x => x.submitNote)
                .Cascade(CascadeMode.Stop)
                .Matches(notePattern)
                .WithMessage("Invalid Submit Note. Must be between 1 and 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.submitNote));

            // Validate cost (optional)
            RuleFor(x => x.cost)
                .GreaterThan(0)
                .WithMessage("Invalid Cost Value. Cost must be a positive integer.")
                .When(x => x.cost.HasValue);
        }
    }
}
