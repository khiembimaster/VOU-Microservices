namespace QuizGame.Domain.ValueObjects
{
    public record ChoiceId
    {
        public Guid Value { get; }
        private ChoiceId(Guid value) => Value = value;
        public static ChoiceId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("ChoiceId cannot be empty.");
            }

            return new ChoiceId(value);
        }
    }
}