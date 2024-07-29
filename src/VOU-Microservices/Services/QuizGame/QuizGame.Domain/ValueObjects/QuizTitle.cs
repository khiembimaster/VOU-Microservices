namespace QuizGame.Domain.ValueObjects
{
    public record QuizTitle
    {
        private const int DefaultLength = 5;
        public string Value { get; }
        private QuizTitle(string value) => Value = value;
        public static QuizTitle Of(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength);

            return new QuizTitle(value);
        }
    }
}