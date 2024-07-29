namespace QuizGame.Domain.ValueObjects
{
    public record AnswerId
    {
        public Guid Value { get; }
        private AnswerId(Guid value) => Value = value;
        public static AnswerId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("AnswerId cannot be empty.");
            }

            return new AnswerId(value);
        }
    }
}