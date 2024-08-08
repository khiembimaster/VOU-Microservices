using Orleans.Concurrency;
using Orleans.Placement;
using Orleans.Timers;

namespace QuizGame.Api.Grains;

public interface IOrchestratorGrain : IGrainWithStringKey
{
    Task StartOrchestrate();
}

[PreferLocalPlacement]
[StatelessWorker]
public class OrchestratorGrain
    (ITimerRegistry timerRegistry, ILogger<OrchestratorGrain> logger)
    : Grain, IOrchestratorGrain
{

    private IGrainTimer? _gameTimer;
    private IGameGrain _gameGrain;

    public Task StartOrchestrate()
    {
        //get the game grain
        var gameCode = this.GetGrainId();
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameCode);

        // Start the LobbyPhase after 3s
        timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: OnLobbyPhase,
            state: gameGrain,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromSeconds(3),
                Period = Timeout.InfiniteTimeSpan,
                KeepAlive = true
            });

        return Task.CompletedTask;
    }

    private async Task OnLobbyPhase(IGameGrain game, CancellationToken cancellationToken)
    {
        Console.WriteLine("Welcoming players");
        // The orchestrator will stay idle for 10 minutes, letting player to join
        timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: OnCountdownStarted,
            state: game,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromSeconds(10),
                Period = Timeout.InfiniteTimeSpan,
                KeepAlive = true
            });
    }

    private async Task OnCountdownStarted(IGameGrain game, CancellationToken cancellationToken)
    {
        // Tell clients to start countdown.
        Console.WriteLine("start count down");
        try
        {
            var gameCode = game.GetPrimaryKeyString();
            var orleansTS = TaskScheduler.Current;
            Task t1 = Task.Run(async () =>
            {
                //Assert.AreNotEqual(orleansTS, TaskScheduler.Current);
                await game.PopCurrentQuestion();
                //Assert.AreNotEqual(orleansTS, TaskScheduler.Current);
            });
            await t1;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        // Start another timer which will tell the MC to introduce the prices, tutorials,... after 10s of countdown 
        
        timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: OnIntroduction,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromSeconds(10),
                Period = Timeout.InfiniteTimeSpan,
                KeepAlive = true
            });
    }

    private Task OnIntroduction(object state, CancellationToken cancellationToken)
    {
        _gameTimer?.Dispose();
        return Task.CompletedTask;
    }

    private async Task OnQuestionPhase(object state, CancellationToken cancellationToken)
    {

    }

    private async Task OnEndgame(object state, CancellationToken cancellationToken)
    {

    }
}
