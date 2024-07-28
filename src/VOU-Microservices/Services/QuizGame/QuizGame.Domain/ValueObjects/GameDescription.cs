namespace QuizGame.Domain.ValueObjects
{
    public record GameDescription
    {
        private const int DefaultLength = 200;
        public string Value { get; }
        private GameDescription(string value) => Value = value;
        public static GameDescription Of(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, DefaultLength);

            return new GameDescription(value);
        }
    }
}