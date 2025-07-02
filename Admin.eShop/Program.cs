using Domain.Interfaces;
using Domain.Models;
using eShop.Admin.Middlewares;
using eShop.Domain.Enums;
using eShop.Infrastructure.Data.Context;
using eShop.Main.Helpers;
using eShop.Main.Validations.Category;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data.Repositories;
using Infrastructure.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(policy => policy.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var singingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]));
var tokenValidationParameters = new TokenValidationParameters()
{
    IssuerSigningKey = singingKey,
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(x => x.DefaultAuthenticateScheme = JwtBearerDefaults
        .AuthenticationScheme)
        .AddJwtBearer(jwt =>
        {
            jwt.TokenValidationParameters = tokenValidationParameters;
        });

builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(SqlUnitOfWork<>));
builder.Services.AddIoCService();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>(ServiceLifetime.Transient);
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    try
    {
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        // Apply any pending migrations
        dbContext.Database.Migrate();

        var uncatgorizedCategory = dbContext.Categories.Any(x => x.Name == "UNCATEGORIZED");
        if (!uncatgorizedCategory)
        {
            var category = new Category("UNCATEGORIZED");

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();
        }


        var uncatgorizedSubcategory = dbContext.Subcategories.Any(x => x.Name == "UNCATEGORIZED");
        if (!uncatgorizedSubcategory)
        {
            var category = dbContext.Categories.Where(x => x.Name == "UNCATEGORIZED").First();

            var subcategory = new Subcategory(category.Id, "UNCATEGORIZED");

            dbContext.Subcategories.Add(subcategory);
            dbContext.SaveChanges();
        }



        var adminExist = dbContext.Users.Any(x => x.Role == Role.Admin);

        if (!adminExist)
        {

            var saltKey = PasswordHasher.GenerateSalt();
            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@example.com",
                Username = "admin",
                Role = Role.Admin,
                PasswordHash = PasswordHasher.HashPassword("Admin@123", saltKey),
                SaltKey = Convert.ToBase64String(saltKey),
                CreatedBy = "Admin",
                Created = DateTime.Now,
                LastModifiedBy = null,
                LastModified = null
            };

            dbContext.Users.Add(adminUser);
            dbContext.SaveChanges();

        }

        var customerExist = dbContext.Users.Any(x => x.Role == Role.Customer);

        if (!customerExist) 
        {
            var saltKey = PasswordHasher.GenerateSalt();
            var adminUser = new User
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "test@example.com",
                Username = "testtest",
                Role = Role.Customer,
                PasswordHash = PasswordHasher.HashPassword("Test@123", saltKey),
                SaltKey = Convert.ToBase64String(saltKey),
                CreatedBy = "Admin",
                Created = DateTime.Now,
                LastModifiedBy = null,
                LastModified = null
            };

            dbContext.Users.Add(adminUser);
            dbContext.SaveChanges();
        }


    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during admin user setup: {ex.Message}");
    }
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("MyPolicy");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
