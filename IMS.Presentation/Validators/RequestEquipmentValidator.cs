using FluentValidation;
using IMS.Application.DTO;

namespace IMS.Presentation.Validators
{
    public class RequestEquipmentValidator : AbstractValidator<RequestEquipmentDTO>
    {
        private readonly string datePattern = @"^\d{4}-\d{2}-\d{2}$";
        private readonly string descriptionPattern = @"^.{1,100}$";

        public RequestEquipmentValidator()
        {
            // Validate equipmentId
            RuleFor(x => x.equipmentId).GreaterThan(0).WithMessage("Invalid Equipment Id.");

            // Validate startDate
            RuleFor(x => x.startDate)
                .LessThan(x => x.endDate)
                .WithMessage("Start Date must be before End Date.")
                .Must(BeAValidDate)
                .WithMessage("Invalid Start Date. The date format must be YYYY-MM-DD.")
                .NotEmpty()
                .WithMessage("Start Date is required.");

            // Validate endDate
            RuleFor(x => x.endDate)
                .GreaterThan(x => x.startDate)
                .WithMessage("End Date must be after Start Date.")
                .Must(BeAValidDate)
                .WithMessage("Invalid End Date. The date format must be YYYY-MM-DD.")
                .NotEmpty()
                .WithMessage("End Date is required.");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default(DateTime);
        }
    }
}
