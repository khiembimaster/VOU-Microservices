using Orleans.Timers;
using QuizGame.Api.Exception;
using QuizGame.Common;
using QuizGame.Common.Message;
using QuizGame.Grains;
using System.Security.Cryptography;

namespace QuizGame.Api.Grains;

public interface IGameGrain : IGrainWithStringKey
{
    Task<string> AddPlayer(Guid playerId);
    Task<Leaderboard> GetLeaderboard();
    Task CreateGame(List<Question> questions);
    Task PopCurrentQuestion();
    Task ClearLeaderboard();
    //Update leaderboard on a player submission
    Task SubmitAnswer(PlayerSubmission submission);
}

public class GameGrain : Grain, IGameGrain
{
    private readonly IPushNotifierGrain _notifier;
    private readonly GameState _state = new GameState();
    private readonly ITimerRegistry _timerRegistry;
    private IGrainTimer _lobbyTicker;
    private IGrainTimer _countdownTicker;

    public GameGrain(ITimerRegistry timerRegistry)
    {
        _timerRegistry = timerRegistry;
        _notifier = GrainFactory.GetGrain<IPushNotifierGrain>(0);// Get the singleton
    }

    public async Task<string> AddPlayer(Guid playerId)
    {
        //Only accept player when Lobby is opening
        if (_state.GameStatus != GameStatus.OnLobbyPhase)
            throw new JoinGameException("Cannot Join Game", $"The Lobby has been closed!");

        var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var player = new Player(playerId, RandomNumberGenerator.GetHexString(6), String.Empty, 0, new List<(string , string, int)>());
        await playerGrain.SetPlayer(player);
        await _notifier.Send(new GameMessage(_state.Code, new ServerMessage($"Player {player.Name} has joined.")));
        return player.Name;
    }

    public Task ClearLeaderboard()
    {
        _state.Leaderboard = new();
        return Task.CompletedTask;
    }

    // TODO: Create an AI Host 
    public Task CreateGame(List<Question> questions)
    {
        _state.Code = this.GetPrimaryKeyString();
        _state.Leaderboard = new Leaderboard
        {
            Entries = new SortedDictionary<int, Guid>(new DescendingComparer<int>())
        };
        _state.Questions = new();
        
        foreach(var question in questions)
        {
            _state.Questions.Enqueue(question);
        }

        _state.Players = new List<Player>();

        var gameCode = this.GetPrimaryKeyString();

        //Start Waiting Clock
        _state.GameStatus = GameStatus.OnLobbyPhase;
        _lobbyTicker = _timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: OnLobbyPhase,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.Zero,
                Period = TimeSpan.FromSeconds(1),
                KeepAlive = true
            });
        return Task.CompletedTask;
    }

    public Task<Leaderboard> GetLeaderboard()
    {
        return Task.FromResult(_state.Leaderboard);
    }

    public async Task PopCurrentQuestion()
    {
        var question = _state.Questions.Dequeue();
        _state.CurrentQuestion = question;
        await _notifier.Send(new GameMessage(_state.Code, question));
    }

    private Task UpdateLeaderboard(int score, Guid player)
    {
        _state.Leaderboard.Entries.Add(score, player);
        
        return Task.CompletedTask;
    }

    private async Task SendLeaderboard()
    {
        var Leaderboard = _state.Leaderboard;
        await _notifier.Send(new GameMessage(_state.Code, Leaderboard));
    }

    public Task SubmitAnswer(PlayerSubmission submission)
    {
        var question = _state.CurrentQuestion;
        var player = _state.Players.FirstOrDefault(x => x.Id.Equals(submission.PlayerId));
        if (player == null) {
            throw new ArgumentException("Player not existed!");
        }
        var point = player.Score;
        
        if (question.IsCorrect(submission.answer))
            point += question.Point;
        
        UpdateLeaderboard(point, submission.PlayerId);
        return Task.CompletedTask;
    }

    private Task OnLobbyPhase(object state, CancellationToken cancellationToken)
    {
        _state.Elapse += TimeSpan.FromSeconds(1);
        var elapse = _state.Elapse;

        // start after 30s (TBD)
        if (elapse.TotalSeconds >= 30)
        {
            _state.GameStatus = GameStatus.OnReadyPlayer;
            _state.Elapse = TimeSpan.FromSeconds(10);
            _notifier.Send(new GameMessage(_state.Code, new ServerMessage($"The game about to start in 10")));
            _countdownTicker = _timerRegistry.RegisterGrainTimer(
                GrainContext,
                callback: Countdown,
                state: this,
                options: new GrainTimerCreationOptions
                {
                    DueTime = TimeSpan.Zero,
                    Period = TimeSpan.FromMilliseconds(10),
                    KeepAlive = true
                });
            _lobbyTicker.Dispose();
        }
        return Task.CompletedTask;
    }

    public async Task Countdown(object state, CancellationToken cancellationToken)
    {
        _state.Elapse -= TimeSpan.FromMilliseconds(10);
        var elapse = _state.Elapse;
        await _notifier.Send(new GameMessage(_state.Code, new ServerMessage($"---- {elapse.TotalMilliseconds} ----")));

        if (elapse.TotalSeconds == 0)
        {
            _countdownTicker.Dispose();
        }
    }
}