namespace QuizGame.Domain.ValueObjects
{
    public record QuizDescription
    {
        private const int DefaultLength = 200;
        public string Value { get; }
        private QuizDescription(string value) => Value = value;
        public static QuizDescription Of(string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, DefaultLength);

            return new QuizDescription(value);
        }
    }
}