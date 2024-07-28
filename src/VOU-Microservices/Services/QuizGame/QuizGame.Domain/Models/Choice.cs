namespace QuizGame.Domain.Models
{
    public class Choice : Entity<ChoiceId>
    {
        public string Content { get; private set; } = default;
        public bool IsCorrect { get; private set; } = default;
    }
}