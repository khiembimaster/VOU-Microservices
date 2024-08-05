namespace QuizGame.Domain;


[GenerateSerializer]
public record Player(Guid Id, string Name, string CurrentAnswer, int Score, IReadOnlyList<(string question, string answer, int point)> History);

