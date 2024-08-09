using Microsoft.AspNetCore.SignalR;
using Orleans.Concurrency;
using QuizGame.Api.Grains;
using QuizGame.Api.Hubs;

namespace QuizGame.Api.Workers;

/// <summary>
/// Periodically updates the <see cref="IHubListGrain"/> implementation with a reference to the local <see cref="RemoteLocationHub"/>.
/// </summary>
[Reentrant]
internal sealed class HubListUpdater : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<HubListUpdater> _logger;
    private readonly ILocalSiloDetails _localSiloDetails;
    private readonly RemoteGameHub _gameBroadcaster;

    public HubListUpdater(
        IGrainFactory grainFactory,
        ILogger<HubListUpdater> logger,
        ILocalSiloDetails localSiloDetails,
        IHubContext<GameHub> hubContext)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        _localSiloDetails = localSiloDetails;
        _gameBroadcaster = new RemoteGameHub(hubContext);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IHubListGrain hubListGrain = _grainFactory.GetGrain<IHubListGrain>(Guid.Empty);
        SiloAddress localSiloAddress = _localSiloDetails.SiloAddress;
        IRemoteGameHub selfReference = _grainFactory.CreateObjectReference<IRemoteGameHub>(_gameBroadcaster);

        // This runs in a loop because the HubListGrain does not use any form of persistence, so if the
        // host which it is activated on stops, then it will lose any internal state.
        // If HubListGrain was changed to use persistence, then this loop could be safely removed.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await hubListGrain.AddHub(localSiloAddress, selfReference);
            }
            catch (OrleansException exception) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(exception, "Error polling location hub list");
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch
                {
                    // Ignore cancellation exceptions, since cancellation is handled by the outer loop.
                }
            }
        }
    }
}
