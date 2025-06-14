using eShop.Main.Requests.Subcategory;
using FluentValidation;

namespace eShop.Main.Validations.Subcategory;

public class CreateSubcategoryRequestValidator : AbstractValidator<CreateSubcategoryRequest>
{
    public CreateSubcategoryRequestValidator()
    {
        RuleFor(x => x.CategoryId)
            .Must(id => id != Guid.Empty)
            .WithMessage("Category Id must be a valid");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subcategory Name is required.")
            .MinimumLength(3).WithMessage("Subcategory Name must be at least 3 characters long.");
    }
}
