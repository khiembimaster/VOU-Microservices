using Microsoft.AspNetCore.SignalR;
using QuizGame.Api.Exception;
using QuizGame.Api.Grains;
using QuizGame.Common.Message;
using System.Security.Claims;

namespace QuizGame.Api.Hubs;

public class GameHub(IGrainFactory grainFactory) : Hub
{
    public override async Task OnConnectedAsync()
    {
        //await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }
    public async Task JoinGame(JoinGameRequest request)
    {
        
        // try player to game grain
        var gameGrain = grainFactory.GetGrain<IGameGrain>(request.Code);
        try
        {
            await gameGrain.AddPlayer(request.PlayerId);
            await Groups.AddToGroupAsync(Context.ConnectionId, request.Code);
            if (Context.UserIdentifier is not null)
                await Clients.User(Context.UserIdentifier).SendAsync("ReceiveServerMessage", $"You have join");
        }
        catch(JoinGameException ex) 
        {
            if (Context.UserIdentifier is not null)
                await Clients.User(Context.UserIdentifier).SendAsync("ReceiveServerMessage", $"{ex.Message}: {ex.Details}");
        }
    }

    public async Task SubmitAnswer(PlayerSubmission submission)
    {
        var gameGrain = grainFactory.GetGrain<IGameGrain>(submission.Code);
        await gameGrain.SubmitAnswer(submission);
    }
}