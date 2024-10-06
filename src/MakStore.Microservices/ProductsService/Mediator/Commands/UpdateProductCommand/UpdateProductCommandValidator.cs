using FluentValidation;

namespace ProductsService.Mediator.Commands.UpdateProductCommand;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(c => c.Name)
            .MinimumLength(4)
            .MaximumLength(128);

        RuleFor(c => c.Description)
            .MaximumLength(512);

        RuleFor(c => c.Price)
            .Must(price => price >= 0);
    }
}