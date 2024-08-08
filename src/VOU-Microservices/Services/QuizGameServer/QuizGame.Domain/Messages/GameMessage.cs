namespace QuizGame.Common.Message;

[Immutable, GenerateSerializer]
public record GameMessage(string Code, object Payload);

[Immutable, GenerateSerializer]
public record class GameBatch(List<GameMessage> Commands);