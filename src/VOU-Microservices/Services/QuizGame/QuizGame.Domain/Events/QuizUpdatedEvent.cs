namespace QuizGame.Domain.Events
{
    public record QuizUpdatedEvent(Quiz quiz) : IDomainEvent;
}