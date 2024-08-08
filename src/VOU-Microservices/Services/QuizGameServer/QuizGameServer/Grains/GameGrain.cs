using Orleans.Timers;
using QuizGame.Common;
using QuizGame.Common.Message;
using QuizGame.Grains;
using System.Security.Cryptography;

namespace QuizGame.Api.Grains;

public interface IGameGrain : IGrainWithStringKey
{
    Task AddPlayer(Guid playerId);
    Task<Leaderboard> GetLeaderboard();
    Task CreateGame(List<Question> questions);
    Task PopCurrentQuestion();
    Task ClearLeaderboard();
    //Update leaderboard on a player submission
    Task UpdateLeaderboard(int score, Guid player);
}

public class GameGrain : Grain, IGameGrain
{
    private readonly IPushNotifierGrain _notifier;
    private readonly GameState _state = new GameState();
    private readonly ITimerRegistry _timerRegistry;
    private IGrainTimer _ticker;
    private IGrainTimer _countdownTicker;

    public GameGrain(ITimerRegistry timerRegistry)
    {
        _timerRegistry = timerRegistry;
        _notifier = GrainFactory.GetGrain<IPushNotifierGrain>(0);// Get the singleton
    }

    public async Task AddPlayer(Guid playerId)
    {
        var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(playerId);
        var player = new Player(playerId, RandomNumberGenerator.GetHexString(6), String.Empty, 0, new List<(string , string, int)>());
        await playerGrain.SetPlayer(player);
        await _notifier.Send(new GameMessage(_state.Code, new ServerMessage($"Player {player.Name} has joined.")));
    }

    public Task ClearLeaderboard()
    {
        _state.Leaderboard = new();
        return Task.CompletedTask;
    }

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
        _ticker = _timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: UpdateTick,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.Zero,
                Period = TimeSpan.FromSeconds(1),
                KeepAlive = true
            });
        return Task.CompletedTask;
    }

    public async Task Countdown(object state, CancellationToken cancellationToken)
    {
        _state.Elapse -= TimeSpan.FromMilliseconds(10);
        var elapse = _state.Elapse;
        await _notifier.Send(new GameMessage(_state.Code, new ServerMessage($"---- {elapse.TotalMilliseconds} ----")));

        if(elapse.TotalSeconds == 0)
        {
            _countdownTicker.Dispose();
        }
    }

    private async Task UpdateTick(object state, CancellationToken cancellationToken)
    {
        _state.Elapse += TimeSpan.FromSeconds(1);
        var elapse = _state.Elapse;

        if (elapse.TotalSeconds == 30)
        {
            await StartCountdown();
        }
    }

    public async Task StartCountdown()
    {
        _state.Elapse = TimeSpan.FromSeconds(10);
        await _notifier.Send(new GameMessage(_state.Code, new ServerMessage($"The game about to start in 10")));
        _ticker.Dispose();
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
    }

    public Task<Leaderboard> GetLeaderboard()
    {
        return Task.FromResult(_state.Leaderboard);
    }

    public async Task PopCurrentQuestion()
    {
        var question = _state.Questions.Dequeue();
        await _notifier.Send(new GameMessage(_state.Code, question));
    }

    public Task UpdateLeaderboard(int score, Guid player)
    {
        _state.Leaderboard.Entries.Add(score, player);
        
        return Task.CompletedTask;
    }

    private async Task SendLeaderboard()
    {
        var Leaderboard = _state.Leaderboard;
        await _notifier.Send(new GameMessage(_state.Code, Leaderboard));
    }
}