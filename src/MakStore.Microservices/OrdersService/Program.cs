using OrdersService;

WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .InitializeApp()
    .ConfigurePipeline()
    .Run();