var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Admin_eShop>("admin-eshop");

builder.AddProject<Projects.PublicApi>("publicapi");

builder.Build().Run();
