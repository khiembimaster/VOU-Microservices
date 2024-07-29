using QuizGame.Domain.Exceptions;
namespace QuizGame.Domain.ValueObjects
{
    public record QuizId
    {
        public Guid Value { get; }
        private QuizId(Guid value) => Value = value;
        public static QuizId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("GameId cannot be empty.");
            }

            return new QuizId(value);
        }
    }
}