namespace BuildingBlocks.Messaging.Events
{
    public record QuizGameStartedEvent(Guid QuizId): IntegrationEvent;
}
