namespace QuizGame.Domain.ValueObjects
{
    public record GameTitle
    {
        private const int DefaultLength = 5;
        public string Value { get; }
        private GameTitle(string value) => Value = value;
        public static GameTitle Of(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength);

            return new GameTitle(value);
        }
    }
}