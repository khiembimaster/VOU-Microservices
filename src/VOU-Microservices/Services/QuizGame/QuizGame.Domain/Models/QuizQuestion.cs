namespace QuizGame.Domain.Models
{
    public class QuizQuestion : Entity<QuizQuestionId>
    {
        internal QuizQuestion(QuizId gameId, QuestionId questionId, int score) 
        { 
            Id = QuizQuestionId.Of(Guid.NewGuid());
            GameId = gameId;
            QuestionId = questionId;
            Score = score;
            //Order = order;
        }

        public QuizId GameId { get; private set; } = default;
        public QuestionId QuestionId { get; private set; } = default;
        public int Score;
    }
}
