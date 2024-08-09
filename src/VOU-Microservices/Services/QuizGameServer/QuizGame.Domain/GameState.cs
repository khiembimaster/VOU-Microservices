namespace QuizGame.Common;

[GenerateSerializer]
public class GameState
{
    [Id(0)]
    public string Code { get; set; }
    [Id(1)]
    public Question CurrentQuestion { get; set; }
    [Id(2)]
    public Queue<Question> Questions { get; set; }
    [Id(3)]
    public List<Player> Players { get; set; }
    [Id(4)]
    public Leaderboard Leaderboard { get; set; } = new();
    [Id(5)]
    public TimeSpan Elapse { get; set; }
    [Id(6)]
    public GameStatus GameStatus { get; set; }
}

public enum GameStatus
{
    OnLobbyPhase = 0,
    OnReadyPlayer = 1,
    QuestionPhase = 2,
    QuizOver = 3,
}

