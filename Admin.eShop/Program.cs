using Domain.Interfaces;
using eShop.Admin.Extensions;
using eShop.Admin.Middlewares;
using eShop.Infrastructure.Data.Context;
using eShop.Main.Validations.Category;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data.Repositories;
using Infrastructure.IoC;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddCors(policy => policy.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(SqlUnitOfWork<>));
builder.Services.AddIoCService();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>(ServiceLifetime.Transient);
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var password = builder.Configuration["SeedAdmin:Password"];
        AppDbContextSeed.SeedAdminUser(dbContext, password);

        AppDbContextSeed.SeedUncategorizedCategory(dbContext);
        AppDbContextSeed.SeedUncategorizedSubcategory(dbContext);
        AppDbContextSeed.SeedTestUser(dbContext);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during data seeding");
    }
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("MyPolicy");

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
