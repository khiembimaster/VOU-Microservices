using Orleans.Concurrency;
using Orleans.Timers;
using QuizGame.Api.Grains;
using QuizGame.Common.Message;

namespace QuizGame.Grains;

public interface IPushNotifierGrain : IGrainWithIntegerKey
{
    ValueTask Send(GameMessage message);
}

[Reentrant]
[StatelessWorker(maxLocalWorkers: 12)]
public class PushNotifierGrain
    (ITimerRegistry timerRegistry, ILogger<PushNotifierGrain> logger)
    : Grain, IPushNotifierGrain
{

    //Use to communicate
    private List<(SiloAddress Host, IRemoteGameHub Hub)> _hubs = new();
    private readonly Queue<GameMessage> _messageQueue = new();
    private Task _flushTask = Task.CompletedTask;
    
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Set up a timer to regularly flush the message queue
        timerRegistry.RegisterGrainTimer(
            GrainContext,
            callback: (state, cancellationToken) => Flush(),
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromMilliseconds(15),
                Period = TimeSpan.FromMilliseconds(15),
            });

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

    private async Task RefreshHubs(object state, CancellationToken cancellationToken)
    {
        IHubListGrain hubListGrain = GrainFactory.GetGrain<IHubListGrain>(Guid.Empty);
        _hubs = await hubListGrain.GetHubs();
    }

    public override async Task OnDeactivateAsync(DeactivationReason deactivationReason, CancellationToken cancellationToken)
    {
        await Flush();
        await base.OnDeactivateAsync(deactivationReason, cancellationToken);
    }

    public ValueTask Send(GameMessage command)
    {
        _messageQueue.Enqueue(command);
        return new(Flush());
    }

    private Task Flush()
    {
        if (_flushTask.IsCompleted)
        {
            _flushTask = FlushInternal();
        }

        return _flushTask;

        async Task FlushInternal()
        {
            const int MaxMessagesPerBatch = 100;
            if (_messageQueue.Count == 0) return;

            while (_messageQueue.Count > 0)
            {
                // Send all messages to all SignalR hubs
                var messagesToSend = new List<GameMessage>(Math.Min(_messageQueue.Count, MaxMessagesPerBatch));
                while (messagesToSend.Count < MaxMessagesPerBatch && _messageQueue.TryDequeue(out GameMessage? msg)) messagesToSend.Add(msg);

                var tasks = new List<Task>(_hubs.Count);
                var batch = new GameBatch(messagesToSend);

                foreach ((SiloAddress Host, IRemoteGameHub Hub) hub in _hubs)
                {
                    tasks.Add(hub.Hub.HandleCommands(batch));
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
