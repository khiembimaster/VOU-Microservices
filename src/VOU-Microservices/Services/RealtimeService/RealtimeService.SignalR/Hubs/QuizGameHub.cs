using Microsoft.AspNetCore.SignalR;

namespace RealtimeService.SignalR.Hubs
{
    public class QuizGameHub : Hub<IQuizGameClient>
    {
        public async Task JoinQuiz()
        {

        }

        public async Task SubmitAnswer()
        {

        }

        public async Task LeaveQuiz()
        {

        }

        public async Task SendMessageToGroup()
        {

        }
    }
}
