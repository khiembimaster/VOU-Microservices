using Microsoft.AspNetCore.SignalR;
using QuizGame.Api.Grains;
using QuizGame.Common.Message;

namespace QuizGame.Api.Hubs;

public class GameHub(IGrainFactory grainFactory) : Hub
{
    public override async Task OnConnectedAsync()
    {
        //await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }
    public async Task JoinGame(JoinGameRequest request)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, request.Code);
        // add player to game grain
        
        var gameGrain = grainFactory.GetGrain<IGameGrain>(request.Code);
        await gameGrain.AddPlayer(request.PlayerId);
    }

    public async Task SubmitAnswer(string gameCode, Guid player, int score)
    {
        var gameGrain = grainFactory.GetGrain<IGameGrain>(gameCode);
        await gameGrain.UpdateLeaderboard(score, player);
    }
}