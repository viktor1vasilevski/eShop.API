using Domain.Models.Base;
using eShop.Domain.Exceptions;

namespace Domain.Models;

public class Product : AuditableBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int UnitQuantity { get; set; }
    public byte[] Image { get; set; }
    public string ImageType { get; set; }


    public Guid SubcategoryId { get; set; }
    public virtual Subcategory? Subcategory { get; set; }

    protected Product() { }
    public Product(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, byte[] imageBytes, string imageType)
    {
        Initialize(name, description, unitPrice, unitQuantity, subcategoryId, imageBytes, imageType);
    }

    public void Update(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, byte[] imageBytes, string imageType) => ApplyChanges(name, description, unitPrice, unitQuantity, subcategoryId, imageBytes, imageType);

    private void ApplyChanges(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, byte[] imageBytes, string imageType)
    {
        Validate(name, description, unitPrice, unitQuantity, subcategoryId, imageBytes, imageType);
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
        Image = imageBytes;
        ImageType = imageType;
    }
    private void Initialize(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, byte[] imageBytes, string imageType)
    {
        Validate(name, description, unitPrice, unitQuantity, subcategoryId, imageBytes, imageType);
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
        Image = imageBytes;
        ImageType = imageType;
    }

    private void Validate(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, byte[] imageBytes, string imageType)
    {
        if (subcategoryId == Guid.Empty)
            throw new DomainValidationException("Subcategory Id cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Name cannot be empty.");

        if (name.Length > 50)
            throw new DomainValidationException("Brand name cannot exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainValidationException("Brand cannot be empty.");

        if (description.Length > 500)
            throw new DomainValidationException("Description cannot exceed 500 characters.");

        if (unitPrice <= 0)
            throw new DomainValidationException("Unit price must be greater than zero.");

        if (unitQuantity <= 0)
            throw new DomainValidationException("Unit quantity must be greater than zero.");

        ValidateImage(imageBytes, imageType);
    }

    private void ValidateImage(byte[]? image, string? imageType)
    {
        if (image is null || image.Length == 0)
            throw new DomainValidationException("Image must be provided.");

        const int maxImageSizeInBytes = 5 * 1024 * 1024; // 5MB
        if (image.Length > maxImageSizeInBytes)
            throw new DomainValidationException("Image size cannot exceed 5MB.");

        if (string.IsNullOrWhiteSpace(imageType))
            throw new DomainValidationException("Image type must be provided.");

        var allowedTypes = new[] { "jpeg", "png", "webp" };
        if (!allowedTypes.Contains(imageType.ToLower()))
            throw new DomainValidationException($"Unsupported image type: {imageType}");
    }
}
