using AuthService;

WebApplication
    .CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .ConfigurePipeline()
    .Run();