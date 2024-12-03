using FluentValidation;
using Api.Dtos.StatusDtos;

public class StatusDtoValidator : AbstractValidator<StatusDto>
{
    public StatusDtoValidator()
    {
        // Перевірка на наявність Id та його коректність
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        
        // Перевірка на наявність Title та його непорожність
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(1, 100).WithMessage("Title must be between 1 and 100 characters.");
    }
}