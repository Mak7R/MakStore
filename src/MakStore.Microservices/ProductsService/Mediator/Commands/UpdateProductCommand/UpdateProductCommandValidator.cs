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
            .MaximumLength(1024);

        RuleFor(c => c.Price)
            .Must(price => price >= 0);
        
        RuleFor(c => c.ResourcesUris)
            .MaximumLength(512);
    }
}