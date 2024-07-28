namespace QuizGame.Domain.ValueObjects
{
    public record LeaderBoardId
    {
        public Guid Value { get; }
        private LeaderBoardId(Guid value) => Value = value;
        public static LeaderBoardId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("LeaderBoardId cannot be empty.");
            }

            return new LeaderBoardId(value);
        }
    }
}