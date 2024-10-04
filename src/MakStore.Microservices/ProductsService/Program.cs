using ProductsService;

WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .InitializeApp()
    .ConfigurePipeline()
    .Run();