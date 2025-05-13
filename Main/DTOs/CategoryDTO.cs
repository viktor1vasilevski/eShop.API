﻿namespace eShop.Main.DTOs;

public class CategoryDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}
