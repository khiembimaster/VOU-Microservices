using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;

namespace QuizGame.Application.QuizGame.EventHandlers.Integration
{
    public class QuizCreatedEventHandler
        (ISender sender)
        : IConsumer<QuizGameRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<QuizGameRegisteredEvent> context)
        {
            //var game = MapToCreateGameCommand(context.Message);

            var command = MapToCreateGameCommand(context);
            await sender.Send(command);
        }


    }
}
