namespace QuizGame.Domain.ValueObjects
{
    public record GameQuestionId
    {
        public Guid Value { get; }
        private GameQuestionId(Guid value) => Value = value;
        public static GameQuestionId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
                throw new DomainException("GameQuestionId cannot be empty.");

            return new GameQuestionId(value);
        }
    }
}