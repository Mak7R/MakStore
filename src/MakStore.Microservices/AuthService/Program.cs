using AuthService.Configuration.Extensions;

WebApplication
    .CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .ConfigurePipeline()
    .Run();