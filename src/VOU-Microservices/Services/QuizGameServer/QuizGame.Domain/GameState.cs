namespace QuizGame.Common;

[GenerateSerializer]
public class GameState
{
    [Id(0)]
    public string Code { get; set; }
    [Id(1)]
    public Queue<Question> Questions { get; set; }
    [Id(2)]
    public List<Player> Players { get; set; }
    [Id(3)]
    public Leaderboard Leaderboard { get; set; } = new();
    [Id(4)]
    public TimeSpan Elapse { get; set; }
}

