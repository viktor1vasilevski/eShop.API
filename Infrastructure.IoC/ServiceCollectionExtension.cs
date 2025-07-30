using eShop.Main.Interfaces;
using eShop.Main.Services;
using Main.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddIoCService(this IServiceCollection services)
    {

        services.AddScoped<IAuthService, AdminAuthService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubcategoryService, SubcategoryService>();
        services.AddScoped<IProductService, ProductService>();
        //services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBasketService, BasketService>();

        return services;
    }
}
