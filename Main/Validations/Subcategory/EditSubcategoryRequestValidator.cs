using eShop.Main.Requests.Subcategory;
using FluentValidation;

namespace eShop.Main.Validations.Subcategory;

public class EditSubcategoryRequestValidator : AbstractValidator<EditSubcategoryRequest>
{
    public EditSubcategoryRequestValidator()
    {
        RuleFor(x => x.CategoryId)
            .Must(id => id != Guid.Empty)
            .WithMessage("Category Id must be a valid");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category Name is required.")
            .MinimumLength(3).WithMessage("Category Name must be at least 3 characters long.");
    }
}
