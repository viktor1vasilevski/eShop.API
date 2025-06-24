using eShop.Main.Requests.Product;
using FluentValidation;

namespace eShop.Main.Validations.Product;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(50).WithMessage("Product name cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Unit price is required.")
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

        RuleFor(x => x.Quantity)
            .NotNull().WithMessage("Unit quantity is required.")
            .GreaterThan(0).WithMessage("Unit quantity must be greater than zero.");

        RuleFor(x => x.SubcategoryId)
            .NotEmpty().WithMessage("Subcategory Id is required.")
            .Must(id => id != Guid.Empty).WithMessage("Subcategory Id must not be an empty GUID.");
    }
}
