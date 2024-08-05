namespace QuizGame.Domain;

[GenerateSerializer]
public class GameState
{
    [Id(0)]
    public Guid Id { get; set; }
    [Id(1)]
    public Queue<Question> Questions { get; set; }
    [Id(2)]
    public List<Player> Players { get; set; }
    [Id(3)]
    public Leaderboard Leaderboard { get; set; } = new();
}

