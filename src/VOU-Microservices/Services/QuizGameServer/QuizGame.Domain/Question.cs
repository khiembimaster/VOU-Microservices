namespace QuizGame.Domain;

[GenerateSerializer]
public record Question(
    int QuestionId, 
    string QuestionText, 
    List<string> Choices, 
    string CorrectAnswer, 
    int Point, 
    int TimeLimit);

