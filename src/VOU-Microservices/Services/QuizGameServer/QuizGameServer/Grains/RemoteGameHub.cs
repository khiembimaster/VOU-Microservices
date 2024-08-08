using Microsoft.AspNetCore.SignalR;
using QuizGame.Api.Hubs;
using QuizGame.Common.Message;
namespace QuizGame.Api.Grains;

public interface IRemoteGameHub : IGrainObserver
{
    public Task HandleCommands(GameBatch batch);
}

public class RemoteGameHub
    : IRemoteGameHub
{
    private readonly IHubContext<GameHub> _hub;
    
    public RemoteGameHub(IHubContext<GameHub> hub) => _hub = hub;
  
    public Task HandleCommands(GameBatch batch)
    {
        foreach (GameMessage message in batch.Commands)
        {
            var code = message.Code;
            var command = "Receive" + message.Payload.GetType().Name;
            var payload = message.Payload;
            try
            {
                _hub.Clients.Group(code).SendAsync(command, payload);
            }catch
            {
                Console.WriteLine("Group {0} not here", code);
            }
        }

        return Task.CompletedTask;
    }
}
