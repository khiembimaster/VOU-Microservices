namespace QuizGame.Common;

[GenerateSerializer]
public record Question(
    string gameCode,
    string QuestionText, 
    List<string> Choices, 
    string CorrectAnswer, 
    int Point, 
    int TimeLimit);

