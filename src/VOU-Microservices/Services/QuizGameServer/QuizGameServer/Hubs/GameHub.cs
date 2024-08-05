using Microsoft.AspNetCore.SignalR;
using QuizGame.Domain;

namespace QuizGame.Api.Hubs;

public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }
    // Method to notify clients of a new question
    public async Task SendQuestion(Guid gameId, Question question)
    {
        await Clients.Group(gameId.ToString()).SendAsync("ReceiveQuestion", question);
    }

    // Method to update clients with the current leaderboard
    public async Task UpdateLeaderboard(Leaderboard leaderboard)
    {
        await Clients.All.SendAsync("ReceiveLeaderboard", leaderboard);
    }

    // Method to update clients with player scores
    public async Task UpdatePlayerScore(Guid playerId, int newScore)
    {
        await Clients.All.SendAsync("ReceivePlayerScore", playerId, newScore);
    }

    // Method to notify clients of AI chat messages
    public async Task SendAiMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveAiMessage", message);
    }
}