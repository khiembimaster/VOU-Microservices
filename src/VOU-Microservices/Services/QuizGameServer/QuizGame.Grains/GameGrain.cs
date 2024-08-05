using QuizGame.Domain;

namespace QuizGame.Grains;

public interface IGameGrain : IGrainWithGuidKey
{
    Task AddPlayer(Player player);
    Task<Leaderboard> GetLeaderboard();
    Task CreateGame(List<Question> questions);
    Task<Question> PopCurrentQuestion();
    Task ClearLeaderboard();
    //Update leaderboard on a player submission
    Task UpdateLeaderboard(int score, Guid player);
}

public class GameGrain : Grain, IGameGrain
{
    private readonly GameState _state = new GameState();
    public async Task AddPlayer(Player player)
    {
        var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(player.Id);
        await playerGrain.SetPlayer(player);

        _state.Players.Add(player);
    }

    public Task ClearLeaderboard()
    {
        _state.Leaderboard = new();

        return Task.CompletedTask;
    }

    public Task CreateGame(List<Question> questions)
    {
        _state.Id = this.GetPrimaryKey();
        _state.Leaderboard = = new(new DescendingComparer<int>()); ;
        foreach(var question in questions)
        {
            _state.Questions.Enqueue(question);
        }
        _state.Players = new List<Player>();

        var gameId = this.GetPrimaryKey();
        var orchestrator = GrainFactory.GetGrain<IOrchestratorGrain>(gameId);
        
        orchestrator.StartOrchestrate();

        return Task.CompletedTask;
    }

    public Task<Leaderboard> GetLeaderboard()
    {
        return Task.FromResult(_state.Leaderboard);
    }

    public Task<Question> PopCurrentQuestion()
    {
        return Task.FromResult(_state.Questions.Dequeue());
    }

    public Task UpdateLeaderboard(int score, Guid player)
    {
        _state.Leaderboard.Entries.Add(score, player);

        return Task.CompletedTask;
    }
}