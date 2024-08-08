using Orleans.Hosting;
using QuizGame.Api.Grains;
using QuizGame.Api.Hubs;
using QuizGame.Api.Workers;
using QuizGame.Common;
using System.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, silo) =>
{
    // In order to support multiple hosts forming a cluster, they must listen on different ports.
    // Use the --InstanceId X option to launch subsequent hosts.
    int instanceId = ctx.Configuration.GetValue<int>("InstanceId");
    silo.UseLocalhostClustering(
        siloPort: 11111 + instanceId,
        gatewayPort: 30000 + instanceId,
        primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11111));
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

app.MapGet("/start/{gameCode}", async (string gameCode, IGrainFactory grainFactory) =>
{
    var game = grainFactory.GetGrain<IGameGrain>(gameCode);
    List<Question> questions = new List<Question>
        {
            new Question(
                gameCode,
                "What is the capital of France?",
                new List<string> { "Paris", "Berlin", "Madrid", "Rome" },
                "Paris",
                10,
                30
            ),
            new Question(
                gameCode,
                "What is the largest planet in our solar system?",
                new List<string> { "Earth", "Mars", "Jupiter", "Saturn" },
                "Jupiter",
                15,
                45
            ),
            new Question(
                gameCode,
                "Who wrote 'To be, or not to be'?",
                new List<string> { "William Shakespeare", "Charles Dickens", "Jane Austen", "Mark Twain" },
                "William Shakespeare",
                20,
                60
            )
        };
    await game.CreateGame(questions);
});

app.MapHub<GameHub>("/game-hub");

app.Run();
