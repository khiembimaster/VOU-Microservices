using QuizGame.Domain.Exceptions;
namespace QuizGame.Domain.ValueObjects
{
    public record GameId
    {
        public Guid Value { get; }
        private GameId(Guid value) => Value = value;
        public static GameId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("GameId cannot be empty.");
            }

            return new GameId(value);
        }
    }
}