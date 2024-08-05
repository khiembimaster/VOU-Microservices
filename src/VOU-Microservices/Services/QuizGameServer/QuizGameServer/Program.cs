using Orleans.Hosting;
using QuizGame.Api.Grains;
using QuizGame.Api.Hubs;
using QuizGame.Api.Workers;
using QuizGame.Domain;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, silo) =>
    {
        //silo.UseLocalhostClustering();
        int instanceId = ctx.Configuration.GetValue<int>("InstanceId");
        silo.UseLocalhostClustering(
            siloPort: 11111 + instanceId,
            gatewayPort: 30000 + instanceId,
            primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111));
        //silo.UseDashboard();
        silo.AddActivityPropagation();
    });
builder.WebHost.UseKestrel((ctx, kestrelOptions) =>
{
    // To avoid port conflicts, each Web server must listen on a different port.
    int instanceId = ctx.Configuration.GetValue<int>("InstanceId");
    kestrelOptions.ListenLocalhost(5001 + instanceId);
});

builder.Services.AddHostedService<HubListUpdater>();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapGet("/start", (IGrainFactory grainFactory) =>
{
    var game = grainFactory.GetGrain<IGameGrain>(Guid.NewGuid());
    var questions = new List<Question>
    {
    };
    game.CreateGame(questions);
    Console.WriteLine("Game end");
});

app.MapHub<GameHub>("/game-hub");

app.Run();
