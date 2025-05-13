var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.eShop_Admin>("eshop-admin");

builder.AddProject<Projects.eShop_PublicApi>("eshop-publicapi");

builder.Build().Run();
