using eShop.Main.Interfaces;
using eShop.Main.Services;
using Main.Interfaces;
using Main.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddIoCService(this IServiceCollection services)
    {

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubcategoryService, SubcategoryService>();
        services.AddScoped<IProductService, ProductService>();
        //services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserService, UserService>();
        //services.AddScoped<IImageService, ImageService>();
        //services.AddScoped<IUserBasketService, UserBasketService>();

        return services;
    }
}
