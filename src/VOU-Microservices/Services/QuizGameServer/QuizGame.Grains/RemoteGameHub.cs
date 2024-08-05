using Microsoft.AspNet.SignalR;
using QuizGame.Domain;

namespace QuizGame.Grains;

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

public class RemoteGameHub(IHubContext<GameHub> hub) : IRemoteGameHub
{
    public ValueTask SendAiMessage(string message)
    {
        throw new NotImplementedException();
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
