using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using QuizGame.Api.Grains;
using QuizGame.Api.Hubs;
using QuizGame.Api.Workers;
using QuizGame.Common;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddAuthentication(options =>
{
    // Identity made Cookie authentication the default.
    // However, we want JWT Bearer Auth to be the default.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Configure the Authority to the expected value for
    // the authentication provider. This ensures the token
    // is appropriately validated.
    options.Authority = "Authority URL"; // TODO: Update URL

    // We have to hook the OnMessageReceived event in order to
    // allow the JWT authentication handler to read the access
    // token from the query string when a WebSocket or 
    // Server-Sent Events request comes in.

    // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
    // due to a limitation in Browser APIs. We restrict it to only calls to the
    // SignalR hub in this code.
    // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
    // for more information about security considerations when using
    // the query string to transmit the access token.
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/hubs/game-hub")))
            {
                // Read the token out of the query string
                context.Token = accessToken;    
            }
            return Task.CompletedTask;
        }
    };
});

// Change to use Name as the user identifier for SignalR
// WARNING: This requires that the source of your JWT token 
// ensures that the Name claim is unique!
// If the Name claim isn't unique, users could receive messages 
// intended for a different user!
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GameHub>("/hubs/game-hub");

app.Run();
