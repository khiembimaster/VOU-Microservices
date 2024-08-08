namespace QuizGame.Common.Message;

[GenerateSerializer]
public record PlayerSubmission(Guid PlayerId, string Code, string answer);
