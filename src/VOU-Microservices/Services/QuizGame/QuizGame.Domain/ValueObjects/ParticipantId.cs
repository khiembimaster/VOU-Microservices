namespace QuizGame.Domain.ValueObjects
{
    public record ParticipantId
    {
        public Guid Value { get; }
        private ParticipantId(Guid value) => Value = value;
        public static ParticipantId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("ParticipantId cannot be empty.");
            }

            return new ParticipantId(value);
        }
    }
}