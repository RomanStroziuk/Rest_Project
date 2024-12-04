using FluentValidation;
using Api.Dtos.StatusDtos;

public class StatusDtoValidator : AbstractValidator<StatusDto>
{
    public StatusDtoValidator()
    {
      
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(1, 100).WithMessage("Title must be between 1 and 100 characters.");
    }
}