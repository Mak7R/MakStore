using MassTransit;
var builder = WebApplication.CreateBuilder(args);

var currentAssembly = typeof(Program).Assembly;

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(currentAssembly);
    x.AddSagaStateMachines(currentAssembly);
    x.AddSagas(currentAssembly);
    x.AddActivities(currentAssembly);
            
    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.Host("rabbitmq.makstore", "/", h => {
            h.Username("rootuser");
            h.Password("DbPass20190502");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run();