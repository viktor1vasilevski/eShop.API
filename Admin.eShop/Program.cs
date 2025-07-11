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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var password = builder.Configuration["SeedAdmin:Password"];
        AppDbContextSeed.SeedAdminUser(dbContext, password);

        AppDbContextSeed.SeedUncategorizedCategory(dbContext);
        AppDbContextSeed.SeedUncategorizedSubcategory(dbContext);
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
