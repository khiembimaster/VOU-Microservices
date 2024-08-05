using Microsoft.AspNetCore.SignalR;
using QuizGame.Api.Hubs;
using QuizGame.Domain;

namespace QuizGame.Api.Grains;

public interface IRemoteGameHub : IGrainObserver
{
    // Method to notify clients of a new question
    public ValueTask SendQuestion(Question question);

    // Method to update clients with the current leaderboard
    public ValueTask UpdateLeaderboard(Leaderboard leaderboard);

    // Method to update clients with player scores
    public ValueTask UpdatePlayerScore(Guid playerId, int newScore);

    // Method to notify clients of AI chat messages
    public ValueTask SendAiMessage(string message);
}

public class RemoteGameHub : IRemoteGameHub
{
    private readonly IHubContext<GameHub> _hub;

    public RemoteGameHub(IHubContext<GameHub> hub)
    {
        _hub = hub;
    }

    public ValueTask SendAiMessage(string message)
    {
        return new(_hub.Clients.All.SendAsync("ReceiveAiMessage", message));
    }

    public ValueTask SendQuestion(Question question)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdateLeaderboard(Leaderboard leaderboard)
    {
        throw new NotImplementedException();
    }

    public ValueTask UpdatePlayerScore(Guid playerId, int newScore)
    {
        throw new NotImplementedException();
    }
}
