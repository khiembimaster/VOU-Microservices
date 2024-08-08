using QuizGame.Common;

namespace QuizGame.Api.Grains;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task SetPlayer(Player player);
    Task SubmitAnswer(string answer);
    Task UpdateScore(int newscore);
    Task<Player> GetPlayer();
    Task<string> GetCurrentAnswer();
}

[GenerateSerializer]
public class PlayerState
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public string Name { get; set; } = string.Empty;
    [Id(2)]
    public string CurrentAnswer { get; set; } = null;

    [Id(3)]
    public int Score = 0;

    [Id(4)]
    public List<(string question, string answer, int point)> History { get; set; } = new();
}

public class PlayerGrain : Grain, IPlayerGrain
{
    private readonly PlayerState _state = new PlayerState();
    public Task<string> GetCurrentAnswer()
    {
        return Task.FromResult(_state.CurrentAnswer);
    }

    public Task<Player> GetPlayer()
    {
        return Task.FromResult(new Player(_state.Id, _state.Name, _state.CurrentAnswer, _state.Score, _state.History.AsReadOnly()));
    }

    public Task SetPlayer(Player player)
    {
        _state.Id = player.Id;
        _state.Name = player.Name;

        return Task.CompletedTask;
    }

    public Task SubmitAnswer(string answer)
    {
        _state.CurrentAnswer = answer;

        return Task.CompletedTask;
    }

    public Task UpdateScore(int newscore)
    {
        _state.Score = newscore;

        return Task.CompletedTask;
    }
}

