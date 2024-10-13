using AuthService;

WebApplication
    .CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .InitializeApp()
    .ConfigurePipeline()
    .Run();