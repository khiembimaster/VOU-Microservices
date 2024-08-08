namespace QuizGame.Common.Message;

[GenerateSerializer]
public record JoinGameRequest(Guid PlayerId, string Code);
