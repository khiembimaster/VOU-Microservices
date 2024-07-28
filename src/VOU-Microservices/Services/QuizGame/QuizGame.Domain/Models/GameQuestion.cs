namespace QuizGame.Domain.Models
{
    public class GameQuestion : Entity<GameQuestionId>
    {
        internal GameQuestion(GameId gameId, QuestionId questionId, int order) 
        { 
            Id = GameQuestionId.Of(Guid.NewGuid());
            GameId = gameId;
            QuestionId = questionId;
            Order = order;
        }

        public GameId GameId { get; private set; } = default;
        public QuestionId QuestionId { get; private set; } = default;
        public int Order;
    }
}
