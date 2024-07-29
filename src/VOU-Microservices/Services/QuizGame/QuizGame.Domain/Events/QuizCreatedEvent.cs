namespace QuizGame.Domain.Events
{
    public record QuizCreatedEvent(Quiz quiz) : IDomainEvent;
}