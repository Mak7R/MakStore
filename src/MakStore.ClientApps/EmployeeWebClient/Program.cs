using EmployeeWebClient;

WebApplication.CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .InitializeApp()
    .ConfigurePipeline()
    .Run();