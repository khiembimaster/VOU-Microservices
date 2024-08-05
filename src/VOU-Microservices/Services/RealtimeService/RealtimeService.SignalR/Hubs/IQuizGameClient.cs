namespace RealtimeService.SignalR.Hubs
{
    public interface IQuizGameClient
    {
        Task ReceiveMessage(string user, string message);
        Task PlayerJoined();
        Task PlayerLeft();
        Task QuizStarted();
        Task QuestionUpdated();
        Task QuizEnded();
        Task AnswerResult();
    }
}
