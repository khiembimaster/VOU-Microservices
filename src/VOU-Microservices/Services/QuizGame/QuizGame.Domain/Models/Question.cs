namespace QuizGame.Domain.Models
{
    public class Question : Aggregate<QuestionId>
    {
        private readonly List<Choice> _choices = new();
        public IReadOnlyList<Choice> Choices => _choices.AsReadOnly();

        public string Description { get; private set; } = default!;
        //public QuestionType QuestionType { get; private set; } = default;
        //public Category Category { get; private set; } = default;
        public Difficulty Difficulty { get; private set; } = default;
        public int TimeLimit { get; private set; } = default;
        
        public static Question Create(QuestionId questionId, GameId gameId, string description, int timeLimit)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            ArgumentOutOfRangeException.ThrowIfLessThan(timeLimit, 10); 

            var question = new Question {
                Id = questionId,
                Description = description,
                TimeLimit = timeLimit,
            };

            return question;
        }
    }
}
