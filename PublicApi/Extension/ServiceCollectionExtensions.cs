using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;

namespace PublicApi.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.SwaggerDocument(c =>
            {
                c.DocumentSettings = s =>
                {
                    s.Title = "My API";
                    s.Version = "v1";
                    s.AddAuth("Bearer", new OpenApiSecurityScheme
                    {
                        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    });
                };
                c.EnableJWTBearerAuth = false;
                c.AutoTagPathSegmentIndex = 0;
                c.ShortSchemaNames = true;
            });
        }
    }
}
