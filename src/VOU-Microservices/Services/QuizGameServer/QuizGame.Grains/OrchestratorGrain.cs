using Microsoft.Extensions.Logging;
using Orleans.Placement;
using Orleans.Timers;

namespace QuizGame.Grains;

public interface IOrchestratorGrain : IGrainWithGuidKey
{
    Task StartOrchestrate();
}

[PreferLocalPlacement]
public class OrchestratorGrain
    (ITimerRegistry timerRegistry, ILogger<OrchestratorGrain> logger)
    : Grain, IOrchestratorGrain
{
    //Use to communicate
    private List<(SiloAddress Host, IRemoteGameHub Hub)> _hubs = new();
    private IGrainTimer? _gameTimer;
    private IGameGrain _gameGrain;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Set up a timer to regularly refresh the hubs, to respond to azure infrastructure changes
        timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: RefreshHubs,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.Zero,
                Period = TimeSpan.FromSeconds(60),
            });

        await base.OnActivateAsync(cancellationToken);
    }

    public Task StartOrchestrate()
    {
        //get the game grain
        var gameCode = this.GetPrimaryKey();
        _gameGrain = GrainFactory.GetGrain<IGameGrain>(gameCode);

        // Start the LobbyPhase after 3s
        _gameTimer = timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: OnLobbyPhase,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromSeconds(3),
                Period = Timeout.InfiniteTimeSpan,
                KeepAlive = true
            });

        return Task.CompletedTask;
    }

    private async Task OnLobbyPhase(object state, CancellationToken cancellationToken)
    {
        Console.WriteLine("Welcoming players");
        _gameTimer?.Dispose();
        // The orchestrator will stay idle for 10 minutes, letting player to join
        _gameTimer = timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: OnCountdownStarted,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromSeconds(10),
                Period = Timeout.InfiniteTimeSpan,
                KeepAlive = true
            });
    }

    private async Task OnCountdownStarted(object state, CancellationToken cancellationToken)
    {

        // Tell clients to start countdown.
        var tasks = new List<Task>(_hubs.Count);
        foreach ((SiloAddress Host, IRemoteGameHub Hub) hub in _hubs)
        {
            tasks.Add(BroadcastAiMessage(hub.Host, hub.Hub, "GetReady", logger));
        }
        await Task.WhenAll(tasks);

        // Start another timer which will tell the MC to introduce the prices, tutorials,... after 10s of countdown 
        _gameTimer?.Dispose();
        _gameTimer = timerRegistry.RegisterGrainTimer(
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

    private async Task RefreshHubs(object state, CancellationToken cancellationToken)
    {
        IHubListGrain hubListGrain = GrainFactory.GetGrain<IHubListGrain>(Guid.Empty);
        _hubs = await hubListGrain.GetHubs();
    }

    private static async Task BroadcastAiMessage(SiloAddress host, IRemoteGameHub hub, string msg, ILogger logger)
    {
        try
        {
            await hub.SendAiMessage(msg);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error boradcasting to host {Host}", host);
        }
    }
}
