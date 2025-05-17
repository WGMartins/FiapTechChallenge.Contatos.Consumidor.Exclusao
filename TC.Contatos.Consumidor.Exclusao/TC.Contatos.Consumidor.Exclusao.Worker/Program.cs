using Domain.Interfaces;
using Infrastructure.RabbitMQ;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UseCase.ContatoUseCase.Remover;
using UseCase.Interfaces;
using Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WorkerService>();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetSection("ConnectionStrings")["ConnectionString"]);
}, ServiceLifetime.Scoped);

builder.Services.AddScoped<IContatoRepository, ContatoRepository>();

builder.Services.AddScoped<IRemoverContatoUseCase, RemoverContatoUseCase>();

#region RabbitMQ

builder.Services.AddSingleton<IMessageConsumer, RabbitMQMessageConsumer>();
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<ConnectionFactory>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
    return new ConnectionFactory
    {
        HostName = settings.HostName,
        UserName = settings.UserName,
        Password = settings.Password,
        VirtualHost = settings.VirtualHost,
    };
});

builder.Services.AddSingleton<Func<Task<IConnection>>>(sp =>
{
    var factory = sp.GetRequiredService<ConnectionFactory>();
    return () => factory.CreateConnectionAsync();
});

#endregion

var host = builder.Build();
host.Run();
