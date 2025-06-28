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

    public Product(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, string image)
    {
        Initialize(name, description, unitPrice, unitQuantity, subcategoryId, image);
    }

    public void Update(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, string image)
    {
        ApplyChanges(name, description, unitPrice, unitQuantity, subcategoryId, image);
    }

    private void Initialize(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, string image)
    {
        var imageBytes = ConvertBase64ToBytes(image);
        var imageType = ExtractImageType(image);

        Validate(name, description, unitPrice, unitQuantity, subcategoryId, imageBytes, imageType);

        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        UnitQuantity = unitQuantity;
        SubcategoryId = subcategoryId;
        Image = imageBytes;
        ImageType = imageType;
    }

    private void ApplyChanges(string name, string description, decimal unitPrice, int unitQuantity, Guid subcategoryId, string image)
    {
        var imageBytes = ConvertBase64ToBytes(image);
        var imageType = ExtractImageType(image);

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

    private void ValidateImage(byte[] imageBytes, string imageType)
    {
        if (imageBytes == null || imageBytes.Length == 0)
            throw new DomainValidationException("Image must be provided.");

        const int maxImageSizeInBytes = 5 * 1024 * 1024; // 5MB
        if (imageBytes.Length > maxImageSizeInBytes)
            throw new DomainValidationException("Image size cannot exceed 5MB.");

        if (string.IsNullOrWhiteSpace(imageType))
            throw new DomainValidationException("Image type must be provided.");

        var allowedTypes = new[] { "jpeg", "png", "webp" };
        if (!allowedTypes.Contains(imageType.ToLower()))
            throw new DomainValidationException($"Unsupported image type: {imageType}");
    }

    private byte[] ConvertBase64ToBytes(string base64String)
    {
        if (string.IsNullOrEmpty(base64String)) return Array.Empty<byte>();

        string base64Data = base64String.Contains("base64,")
            ? base64String.Substring(base64String.IndexOf("base64,") + 7)
            : base64String;

        return Convert.FromBase64String(base64Data);
    }

    private string ExtractImageType(string base64String)
    {
        if (string.IsNullOrEmpty(base64String)) return string.Empty;

        var parts = base64String.Split(';');
        if (parts.Length == 0 || !parts[0].Contains("/")) return string.Empty;

        var typeParts = parts[0].Split('/');
        return typeParts.Length > 1 ? typeParts[1] : string.Empty;
    }
}
